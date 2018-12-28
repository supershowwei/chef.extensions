using System.Web;
using System.Web.Mvc;

namespace Chef.Extensions.Controller
{
    internal static class ActionResultExtension
    {
        public static ActionResult SetCacheHeader(this ActionResult me, HttpResponseBase response, string etag)
        {
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.Headers["ETag"] = etag;

            return me;
        }
    }
}