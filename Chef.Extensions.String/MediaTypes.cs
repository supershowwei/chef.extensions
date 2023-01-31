using System.Collections.Generic;

namespace Chef.Extensions.String
{
    // https://www.iana.org/assignments/media-types/media-types.xhtml
    public class MediaTypes
    {
        private static readonly Dictionary<string, string> Types = new Dictionary<string, string>
                                                                   {
                                                                       ["json"] = "application/json",
                                                                       ["gif"] = "image/gif",
                                                                       ["jpeg"] = "image/jpeg",
                                                                       ["png"] = "image/png",
                                                                       ["css"] = "text/css",
                                                                       ["csv"] = "text/csv",
                                                                       ["html"] = "text/html",
                                                                       ["javascript"] = "text/javascript",
                                                                       ["plain"] = "text/plain",
                                                                       ["xml"] = "text/xml"
                                                                   };

        public static string Json => GetMediaType("json");

        public static string Html => GetMediaType("html");

        public static string Plain => GetMediaType("plain");

        public static string GetMediaType(string name)
        {
            return Types[name];
        }
    }
}