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
            if (NotModified(me.Request.Headers["If-None-Match"], me.HttpContext.Cache, out var requestChecksum))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotModified);
            }

            var viewEngineResult = FindView(me, viewName);
            var viewPath = ((RazorView)viewEngineResult.View).ViewPath;

            string cacheKey;
            CacheView cacheView;

            if (MustRefresh(me.Request.Headers["Cache-Control"], out var deltaSeconds))
            {
                var graduation = GetGraduation(DateTime.Now.AddSeconds(deltaSeconds), duration);
                cacheKey = MD5.Hash(viewPath, graduation.ToString("yyyy-MM-dd HH:mm:ss"));

                cacheView = new CacheView(Render(me, viewEngineResult, model));

                TryCache(cacheKey, cacheView, graduation.AddSeconds(duration), me.HttpContext.Cache, viewPath);
            }
            else
            {
                var graduation = GetGraduation(DateTime.Now, duration);
                cacheKey = MD5.Hash(viewPath, graduation.ToString("yyyy-MM-dd HH:mm:ss"));

                if ((cacheView = me.HttpContext.Cache[cacheKey] as CacheView) == null)
                {
                    cacheView = new CacheView(Render(me, viewEngineResult, model));

                    TryCache(cacheKey, cacheView, graduation.AddSeconds(duration), me.HttpContext.Cache, viewPath);
                }
            }

            viewEngineResult.ViewEngine.ReleaseView(me.ControllerContext, viewEngineResult.View);

            me.Response.Headers["ETag"] = $"{cacheKey}-{cacheView.Checksum}";

            if (!string.IsNullOrEmpty(requestChecksum) && requestChecksum.Equals(cacheView.Checksum))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotModified);
            }

            return new ContentResult { Content = cacheView.Output, ContentType = "text/html" };
        }

        private static bool NotModified(string ifNoneMatch, Cache container, out string requestChecksum)
        {
            requestChecksum = null;

            if (string.IsNullOrEmpty(ifNoneMatch)) return false;

            var match = Regex.Match(ifNoneMatch, "([^-]+)-(.+)");

            if (!match.Success) return false;

            requestChecksum = match.Groups[2].Value;

            var cacheView = container[match.Groups[1].Value] as CacheView;

            return cacheView != null && requestChecksum.Equals(cacheView.Checksum);
        }

        private static bool MustRefresh(string cacheControl, out int deltaSeconds)
        {
            deltaSeconds = 0;

            if (string.IsNullOrEmpty(cacheControl)) return false;

            var match = Regex.Match(cacheControl, @"must-refresh=?(\d*)");

            if (!match.Success) return false;

            if (match.Groups.Count > 1) int.TryParse(match.Groups[1].Value, out deltaSeconds);

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

        private static void TryCache(
            string key,
            CacheView value,
            DateTime absoluteExpiration,
            Cache container,
            string lockedKey)
        {
            object locked;
            if (Monitor.TryEnter(locked = ObjectLocker.Instance.GetLockObject(lockedKey)))
            {
                try
                {
                    container.Insert(key, value, null, absoluteExpiration, Cache.NoSlidingExpiration);
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

        private static DateTime GetGraduation(DateTime time, int duration)
        {
            var dayOfSecond = Convert.ToInt32(time.Subtract(time.Date).TotalSeconds);

            return DateTime.Now.Date.AddSeconds(dayOfSecond - dayOfSecond % duration);
        }
    }
}