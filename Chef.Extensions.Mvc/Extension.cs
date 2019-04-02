using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Web.Mvc;
using Chef.Extensions.Mvc.Cachable;
using Chef.Extensions.Mvc.Extensions;
using Chef.Extensions.Mvc.Helpers;

namespace Chef.Extensions.Mvc
{
    public static class Extension
    {
        public static void HeatViews(this Controller me)
        {
            var root = me.Server.MapPath("~/");

            var files = new[] { "Area", "Views" }.Select(x => Path.Combine(root, x))
                .Where(x => Directory.Exists(x))
                .SelectMany(x => Directory.GetFiles(x, "*.cshtml", SearchOption.AllDirectories))
                .OrderBy(x => Path.GetFileName(x).StartsWith("_") ? 0 : 1)
                .GroupBy(x => Path.GetDirectoryName(x))
                .Select(g => g.First());

            foreach (var file in files)
            {
                var viewName = $"~/{file.Replace(root, string.Empty).Replace("\\", "/")}";
                var viewEngineResult = ViewEngines.Engines.FindView(me.ControllerContext, viewName, string.Empty);

                try
                {
                    viewEngineResult.View.Render(
                        new ViewContext(
                            me.ControllerContext,
                            viewEngineResult.View,
                            me.ViewData,
                            me.TempData,
                            TextWriter.Null),
                        TextWriter.Null);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public static HttpStatusCodeResult HttpOK(this Controller me)
        {
            return HttpStatus(me, HttpStatusCode.OK);
        }

        public static HttpStatusCodeResult HttpNotModified(this Controller me)
        {
            return HttpStatus(me, HttpStatusCode.NotModified);
        }

        public static HttpStatusCodeResult HttpBadRequest(this Controller me)
        {
            return HttpStatus(me, HttpStatusCode.BadRequest);
        }

        public static HttpStatusCodeResult HttpUnauthorized(this Controller me)
        {
            return HttpStatus(me, HttpStatusCode.Unauthorized);
        }

        public static HttpStatusCodeResult HttpForbidden(this Controller me)
        {
            return HttpStatus(me, HttpStatusCode.Forbidden);
        }

        public static HttpStatusCodeResult HttpInternalServerError(this Controller me)
        {
            return HttpStatus(me, HttpStatusCode.InternalServerError);
        }

        public static HttpStatusCodeResult HttpStatus(this Controller me, HttpStatusCode statusCode)
        {
            return new HttpStatusCodeResult(statusCode);
        }

        public static HttpStatusCodeResult HttpStatus(this Controller me, int statusCode)
        {
            return new HttpStatusCodeResult(statusCode);
        }

        public static JsonNetResult Jsonet(this Controller me, object data)
        {
            return Jsonet(me, data, null, null);
        }

        public static JsonNetResult Jsonet(this Controller me, object data, string contentType)
        {
            return Jsonet(me, data, contentType, null);
        }

        public static JsonNetResult Jsonet(
            this Controller me,
            object data,
            string contentType,
            Encoding contentEncoding)
        {
            return new JsonNetResult { Data = data, ContentType = contentType, ContentEncoding = contentEncoding };
        }

        public static ActionResult CacheView(this Controller me, int duration = 900)
        {
            return CacheView(me, null, null, duration);
        }

        public static ActionResult CacheView(this Controller me, string viewName, int duration = 900)
        {
            return CacheView(me, viewName, null, duration);
        }

        public static ActionResult CacheView(this Controller me, object model, int duration = 900)
        {
            return CacheView(me, null, model, duration);
        }

        public static ActionResult CacheView(this Controller me, string viewName, object model, int duration = 900)
        {
            if (model != null) me.ViewData.Model = model;

            var viewEngineResult = FindView(me, viewName);

            var cacheKey = MD5.Hash(((RazorView)viewEngineResult.View).ViewPath);
            CacheView cacheView;

            if (MustRefresh(me.Request.Headers["Cache-Control"], out var prolonged))
            {
                cacheView = RenderAndCacheForcibly(me, viewEngineResult.View, cacheKey, duration + prolonged);
            }
            else
            {
                cacheView = RenderAndCache(me, viewEngineResult.View, cacheKey, duration);

                if (NotModified(me.ViewBag.CacheViewIdentity, cacheKey, cacheView.Checksum))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotModified).SetCacheHeader(
                        me.Response,
                        $"{cacheKey}-{cacheView.Checksum}");
                }
            }

            viewEngineResult.ViewEngine.ReleaseView(me.ControllerContext, viewEngineResult.View);

            return new ContentResult { Content = cacheView.Output, ContentType = "text/html" }.SetCacheHeader(
                me.Response,
                $"{cacheKey}-{cacheView.Checksum}");
        }

        public static T GetParameter<T>(this ActionExecutingContext me, string name)
        {
            if (me.ActionParameters.ContainsKey(name)) return (T)me.ActionParameters[name];

            if (me.RouteData.Values.ContainsKey(name))
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));

                return (T)converter.ConvertFrom(me.RouteData.Values[name]);
            }

            return default(T);
        }

        public static bool ContainsParameter(this ActionExecutingContext me, string name)
        {
            if (me.ActionParameters.ContainsKey(name)) return true;
            if (me.RouteData.Values.ContainsKey(name)) return true;

            return false;
        }

        private static ViewEngineResult FindView(Controller controller, string viewName)
        {
            viewName = string.IsNullOrEmpty(viewName)
                           ? controller.ControllerContext.RouteData.GetRequiredString("action")
                           : viewName;

            return controller.ViewEngineCollection.FindView(controller.ControllerContext, viewName, string.Empty);
        }

        private static bool MustRefresh(string cacheControl, out int prolonged)
        {
            prolonged = 0;

            if (string.IsNullOrEmpty(cacheControl)) return false;

            var match = Regex.Match(cacheControl, @"must-refresh=?(\d*)");

            if (!match.Success) return false;

            if (match.Groups.Count > 1) int.TryParse(match.Groups[1].Value, out prolonged);

            return true;
        }

        private static CacheView RenderAndCacheForcibly(
            Controller controller,
            IView view,
            string cacheKey,
            int duration)
        {
            CacheView cacheView;

            lock (ObjectLocker.Instance.GetLockObject(cacheKey))
            {
                cacheView = new CacheView(Render(controller, view));

                controller.HttpContext.Cache.Insert(
                    cacheKey,
                    cacheView,
                    null,
                    DateTime.Now.AddSeconds(duration),
                    Cache.NoSlidingExpiration);
            }

            return cacheView;
        }

        private static CacheView RenderAndCache(Controller controller, IView view, string cacheKey, int duration)
        {
            CacheView cacheView;

            if ((cacheView = controller.HttpContext.Cache[cacheKey] as CacheView) == null)
            {
                lock (ObjectLocker.Instance.GetLockObject(cacheKey))
                {
                    if ((cacheView = controller.HttpContext.Cache[cacheKey] as CacheView) == null)
                    {
                        cacheView = new CacheView(Render(controller, view));

                        controller.HttpContext.Cache.Insert(
                            cacheKey,
                            cacheView,
                            null,
                            DateTime.Now.AddSeconds(duration),
                            Cache.NoSlidingExpiration);
                    }
                }
            }

            return cacheView;
        }

        private static string Render(Controller controller, IView view)
        {
            var writer = new StringWriter();

            view.Render(
                new ViewContext(controller.ControllerContext, view, controller.ViewData, controller.TempData, writer),
                writer);

            // TODO: Replace \r\n
            return writer.ToString();
        }

        private static bool NotModified(CacheViewIdentity identity, string cacheKey, string checksum)
        {
            if (identity == null) return false;
            if (!identity.Checksum.Equals(checksum)) return false;
            if (!identity.CacheKey.Equals(cacheKey)) return false;

            return true;
        }
    }
}