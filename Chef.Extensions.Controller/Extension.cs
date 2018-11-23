using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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

        public static ActionResult CacheableView(this System.Web.Mvc.Controller me, int duration = 900)
        {
            return CacheableView(me, null, null, duration);
        }

        public static ActionResult CacheableView(this System.Web.Mvc.Controller me, string viewName, int duration = 900)
        {
            return CacheableView(me, viewName, null, duration);
        }

        public static ActionResult CacheableView(this System.Web.Mvc.Controller me, object model, int duration = 900)
        {
            return CacheableView(me, null, model, duration);
        }

        public static ActionResult CacheableView(
            this System.Web.Mvc.Controller me,
            string viewName,
            object model,
            int duration = 900)
        {
            var etag = me.Request.Headers["If-None-Match"];

            if (!string.IsNullOrEmpty(etag) && me.HttpContext.Cache[etag] != null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotModified);
            }

            var viewEngineResult = FindView(me, viewName);
            var viewPath = ((RazorView)viewEngineResult.View).ViewPath;
            var graduation = GetCurrentGraduation(duration);

            etag = Hash(viewPath, graduation.ToString("yyyy-MM-dd HH:mm:ss"));

            var cacheControl = me.Request.Headers["Cache-Control"];
            if (!string.IsNullOrEmpty(cacheControl) && cacheControl.Contains("must-refresh"))
            {
                me.HttpContext.Cache.Remove(etag);
            }

            var output = (string)me.HttpContext.Cache[etag];
            if (string.IsNullOrEmpty(output))
            {
                output = Render(me, viewEngineResult, model);

                TryCache(etag, output, graduation.AddSeconds(duration), me.HttpContext.Cache, viewPath);
            }

            viewEngineResult.ViewEngine.ReleaseView(me.ControllerContext, viewEngineResult.View);

            me.Response.Headers["ETag"] = etag;

            return new ContentResult { Content = output, ContentType = "text/html" };
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

            return writer.ToString();
        }

        private static void TryCache(
            string key,
            string value,
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

        private static DateTime GetCurrentGraduation(int duration)
        {
            var dayOfSecond = Convert.ToInt32(DateTime.Now.Subtract(DateTime.Now.Date).TotalSeconds);

            return DateTime.Now.Date.AddSeconds(dayOfSecond - dayOfSecond % duration);
        }

        private static string Hash(string value, string salt)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(value, salt)));

                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }
    }
}