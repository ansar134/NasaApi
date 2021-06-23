using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NasaApi.Exceptions
{
    /// <summary>
    /// This is a custom exception that is thrown with the status code and response message in case of
    /// some exception while getting response back from API
    /// </summary>
    public class NasaApiResponseException: Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; }

        public NasaApiResponseException(HttpStatusCode statusCode, string message):base(message)
        {
            StatusCode = statusCode;
            Message = message;
        }


        public string ToJsonString()
        {
            return new JObject(new JProperty("StatusCode", StatusCode),
                new JProperty("Message", Message)).ToString();
        }
    }
}
