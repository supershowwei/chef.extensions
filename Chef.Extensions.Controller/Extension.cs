using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Web.Mvc;

namespace Chef.Extensions.Controller
{
    public static class Extension
    {
        public static void HeatViews(this System.Web.Mvc.Controller me)
        {
            var root = me.Server.MapPath("~/");

            var files = new[] { "Area", "Views" }.Select(x => Path.Combine(root, x))
                .Where(x => Directory.Exists(x))
                .SelectMany(x => Directory.GetFiles(x, "*.cshtml", SearchOption.AllDirectories))
                .GroupBy(x => Path.GetDirectoryName(x))
                .Select(g => g.First())
                .OrderBy(x => Path.GetFileName(x).StartsWith("_") ? 0 : 1)
                .ThenBy(x => x.Contains("Shared") ? 0 : 1);

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

        public static JsonNetResult Jsonet(this System.Web.Mvc.Controller me, object data)
        {
            return Jsonet(me, data, null, null, JsonRequestBehavior.DenyGet);
        }

        public static JsonNetResult Jsonet(this System.Web.Mvc.Controller me, object data, string contentType)
        {
            return Jsonet(me, data, contentType, null, JsonRequestBehavior.DenyGet);
        }

        public static JsonNetResult Jsonet(
            this System.Web.Mvc.Controller me,
            object data,
            string contentType,
            Encoding contentEncoding)
        {
            return Jsonet(me, data, contentType, contentEncoding, JsonRequestBehavior.DenyGet);
        }

        public static JsonNetResult Jsonet(this System.Web.Mvc.Controller me, object data, JsonRequestBehavior behavior)
        {
            return Jsonet(me, data, null, null, behavior);
        }

        public static JsonNetResult Jsonet(
            this System.Web.Mvc.Controller me,
            object data,
            string contentType,
            JsonRequestBehavior behavior)
        {
            return Jsonet(me, data, contentType, null, behavior);
        }

        public static JsonNetResult Jsonet(
            this System.Web.Mvc.Controller me,
            object data,
            string contentType,
            Encoding contentEncoding,
            JsonRequestBehavior behavior)
        {
            return new JsonNetResult
                   {
                       Data = data,
                       ContentType = contentType,
                       ContentEncoding = contentEncoding,
                       JsonRequestBehavior = behavior
                   };
        }

        public static ActionResult CacheView(this System.Web.Mvc.Controller me, int duration = 900)
        {
            return CacheView(me, null, null, duration);
        }

        public static ActionResult CacheView(this System.Web.Mvc.Controller me, string viewName, int duration = 900)
        {
            return CacheView(me, viewName, null, duration);
        }

        public static ActionResult CacheView(this System.Web.Mvc.Controller me, object model, int duration = 900)
        {
            return CacheView(me, null, model, duration);
        }

        public static ActionResult CacheView(
            this System.Web.Mvc.Controller me,
            string viewName,
            object model,
            int duration = 900)
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
                    return new HttpStatusCodeResult(HttpStatusCode.NotModified);
                }

                me.Response.Headers["ETag"] = $"{cacheKey}-{cacheView.Checksum}";
            }

            viewEngineResult.ViewEngine.ReleaseView(me.ControllerContext, viewEngineResult.View);

            return new ContentResult { Content = cacheView.Output, ContentType = "text/html" };
        }

        private static ViewEngineResult FindView(System.Web.Mvc.Controller controller, string viewName)
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
            System.Web.Mvc.Controller controller,
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

        private static CacheView RenderAndCache(
            System.Web.Mvc.Controller controller,
            IView view,
            string cacheKey,
            int duration)
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

        private static string Render(System.Web.Mvc.Controller controller, IView view)
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