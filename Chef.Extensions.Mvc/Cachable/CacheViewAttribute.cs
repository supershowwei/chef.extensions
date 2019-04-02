using System.Net;
using System.Web.Caching;
using System.Web.Mvc;
using Chef.Extensions.Mvc.Extensions;

namespace Chef.Extensions.Mvc.Cachable
{
    public class CacheViewAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;

            var requestIdentity = CacheViewIdentity.Create(httpContext.Request.Headers["If-None-Match"]);

            filterContext.Controller.ViewBag.CacheViewIdentity = requestIdentity;

            if (NotModified(requestIdentity, httpContext.Cache, out var modifiedCacheView))
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.NotModified).SetCacheHeader(
                    httpContext.Response,
                    $"{requestIdentity.CacheKey}-{requestIdentity.Checksum}");

                return;
            }

            if (modifiedCacheView != null)
            {
                filterContext.Result =
                    new ContentResult { Content = modifiedCacheView.Output, ContentType = "text/html" }.SetCacheHeader(
                        httpContext.Response,
                        $"{requestIdentity.CacheKey}-{modifiedCacheView.Checksum}");

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