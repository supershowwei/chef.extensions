using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Caching;
using System.Web.Mvc;

namespace Chef.Extensions.Controller
{
    public static class Extension
    {
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
            var requestIdentity = GenerateIdentity(me.Request.Headers["If-None-Match"]);

            if (NotModified(requestIdentity, me.HttpContext.Cache, out var modifiedCacheView))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotModified);
            }

            if (modifiedCacheView != null)
            {
                me.Response.Headers["ETag"] = $"{requestIdentity.CacheKey}-{modifiedCacheView.Checksum}";

                return new ContentResult { Content = modifiedCacheView.Output, ContentType = "text/html" };
            }

            var viewEngineResult = FindView(me, viewName);
            var viewPath = ((RazorView)viewEngineResult.View).ViewPath;

            var cacheKey = MD5.Hash(viewPath);
            CacheView cacheView;

            if (MustRefresh(me.Request.Headers["Cache-Control"], out var prolongedDuration))
            {
                cacheView = new CacheView(Render(me, viewEngineResult, model));

                TryCache(cacheKey, cacheView, duration + prolongedDuration, me.HttpContext.Cache, viewPath);
            }
            else
            {
                if ((cacheView = me.HttpContext.Cache[cacheKey] as CacheView) == null)
                {
                    cacheView = new CacheView(Render(me, viewEngineResult, model));

                    TryCache(cacheKey, cacheView, duration, me.HttpContext.Cache, viewPath);
                }

                me.Response.Headers["ETag"] = $"{cacheKey}-{cacheView.Checksum}";

                if (requestIdentity != null && requestIdentity.Checksum.Equals(cacheView.Checksum))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotModified);
                }
            }

            viewEngineResult.ViewEngine.ReleaseView(me.ControllerContext, viewEngineResult.View);

            return new ContentResult { Content = cacheView.Output, ContentType = "text/html" };
        }

        private static CacheViewIdentity GenerateIdentity(string ifNoneMatch)
        {
            if (string.IsNullOrEmpty(ifNoneMatch)) return null;

            var match = Regex.Match(ifNoneMatch, "([^-]+)-(.+)");

            if (!match.Success) return null;

            return new CacheViewIdentity { CacheKey = match.Groups[1].Value, Checksum = match.Groups[2].Value };
        }

        private static bool NotModified(CacheViewIdentity identity, Cache container, out CacheView modifiedCacheView)
        {
            modifiedCacheView = null;

            if (identity == null) return false;

            var cacheView = container[identity.CacheKey] as CacheView;

            if (cacheView == null) return false;

            if (identity.Checksum.Equals(cacheView.Checksum)) return true;

            modifiedCacheView = cacheView;

            return false;
        }

        private static bool MustRefresh(string cacheControl, out int prolongedDuration)
        {
            prolongedDuration = 0;

            if (string.IsNullOrEmpty(cacheControl)) return false;

            var match = Regex.Match(cacheControl, @"must-refresh=?(\d*)");

            if (!match.Success) return false;

            if (match.Groups.Count > 1) int.TryParse(match.Groups[1].Value, out prolongedDuration);

            return true;
        }

        private static ViewEngineResult FindView(System.Web.Mvc.Controller controller, string viewName)
        {
            viewName = string.IsNullOrEmpty(viewName)
                           ? controller.ControllerContext.RouteData.GetRequiredString("action")
                           : viewName;

            return controller.ViewEngineCollection.FindView(controller.ControllerContext, viewName, string.Empty);
        }

        private static string Render(
            System.Web.Mvc.Controller controller,
            ViewEngineResult viewEngineResult,
            object model)
        {
            if (model != null) controller.ViewData.Model = model;

            var writer = new StringWriter();

            viewEngineResult.View.Render(
                new ViewContext(
                    controller.ControllerContext,
                    viewEngineResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer),
                writer);

            // TODO: Replace \r\n
            return writer.ToString();
        }

        private static void TryCache(string key, CacheView value, int duration, Cache container, string lockedKey)
        {
            object locked;
            if (Monitor.TryEnter(locked = ObjectLocker.Instance.GetLockObject(lockedKey)))
            {
                try
                {
                    container.Insert(key, value, null, DateTime.Now.AddSeconds(duration), Cache.NoSlidingExpiration);
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    Monitor.Exit(locked);
                }
            }
        }
    }
}