using System.Text;
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

        public static JsonNetResult Jsonet(this System.Web.Mvc.Controller me, object data, string contentType, Encoding contentEncoding)
        {
            return Jsonet(me, data, contentType, contentEncoding, JsonRequestBehavior.DenyGet);
        }

        public static JsonNetResult Jsonet(this System.Web.Mvc.Controller me, object data, JsonRequestBehavior behavior)
        {
            return Jsonet(me, data, null, null, behavior);
        }

        public static JsonNetResult Jsonet(this System.Web.Mvc.Controller me, object data, string contentType, JsonRequestBehavior behavior)
        {
            return Jsonet(me, data, contentType, null, behavior);
        }

        public static JsonNetResult Jsonet(this System.Web.Mvc.Controller me, object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
                   {
                       Data = data,
                       ContentType = contentType,
                       ContentEncoding = contentEncoding,
                       JsonRequestBehavior = behavior
                   };
        }
    }
}