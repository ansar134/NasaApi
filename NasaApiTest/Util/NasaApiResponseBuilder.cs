using NasaApi.Models;
using NasaApi.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace NasaApiTest.Util
{
    public class NasaApiResponseBuilder
    {
        public static StringContent OkResponse = BuildOkResponse();
        public static StringContent UnauthorizedResponse = BuildUnauthorizedResponse();
        public static StringContent BadRequestResponse = BuildBadRequestResponse();
        public static StringContent InternalErrorResponse = BuildInternalErrorResponse();

        private static StringContent BuildOkResponse()
        {
            var response = new NasaAstroPicModel
            {
                
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Explanation = "A very long text",
                Title = "Title",
                Url = "https://apod.nasa.gov/apod/image/2105/AgCar_HubbleSchmidt_960.jpg",
                HdUrl = "https://apod.nasa.gov/apod/image/2105/AgCar_HubbleSchmidt_2212.jpg",
                Media_Type = "image"

            };
            
            var json = JsonSerializer.Serialize(response);
            return new StringContent(json);
        }

        private static StringContent BuildOkResponseWithVideoMediaType()
        {
            var response = new NasaAstroPicModel
            {

                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Explanation = "A very long text",
                Title = "Title",
                Url = "https://www.youtube.com/embed/eMmw7MqsEUo?rel=0",
                Media_Type = "video"

            };

            var json = JsonSerializer.Serialize(response);
            return new StringContent(json);
        }

       

        private static StringContent BuildUnauthorizedResponse()
        {
            var json = JsonSerializer.Serialize(new { Code = 401, Message = "Invalid Api key." });
            return new StringContent(json);
        }

        private static StringContent BuildBadRequestResponse()
        {
            var json = JsonSerializer.Serialize(new { Code = 400, Message = "Date must be between Jun 16, 1995 and today." });
            return new StringContent(json);
        }

        private static StringContent BuildInternalErrorResponse()
        {
            var json = JsonSerializer.Serialize(new { Code = 500, Message = "Internal Error." });
            return new StringContent(json);
        }


        



    }
}
