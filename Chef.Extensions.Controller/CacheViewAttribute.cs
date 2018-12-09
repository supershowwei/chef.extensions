using System.Net;
using System.Web.Caching;
using System.Web.Mvc;

namespace Chef.Extensions.Controller
{
    public class CacheViewAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;

            var requestIdentity = CacheViewIdentity.Create(httpContext.Request.Headers["If-None-Match"]);

            if (NotModified(requestIdentity, httpContext.Cache, out var modifiedCacheView))
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.NotModified);

                return;
            }

            if (modifiedCacheView != null)
            {
                httpContext.Response.Headers["ETag"] = $"{requestIdentity.CacheKey}-{modifiedCacheView.Checksum}";

                filterContext.Result = new ContentResult { Content = modifiedCacheView.Output, ContentType = "text/html" };

                return;
            }

            base.OnActionExecuting(filterContext);
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
    }
}