using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Chef.Extensions.Controller
{
    public class JsonNetResult : JsonResult
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(this.ContentType) ? this.ContentType : "application/json";

            if (this.ContentEncoding != null) response.ContentEncoding = this.ContentEncoding;

            if (this.Data != null)
            {
                var writer = new JsonTextWriter(response.Output);
                var serializer = JsonSerializer.Create(SerializerSettings);

                serializer.Serialize(writer, this.Data);
                writer.Flush();
            }
        }
    }
}