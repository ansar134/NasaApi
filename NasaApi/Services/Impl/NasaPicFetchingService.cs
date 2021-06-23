using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NasaApi.Models;
using NasaApi.Models.DTOs;
using NasaApi.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NasaApi.Exceptions;

namespace NasaApi.Services.Impl
{
    /// <summary>
    /// Implementation of the service that is actually going to fetch the data from the Nasa API
    /// </summary>
    public class NasaPicFetchingService : INasaPicFetchingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly NasaApiDbContext _nasaApiDbContext;
        private readonly IConfiguration _configuration;

        
        public NasaPicFetchingService(NasaApiDbContext dbContext, IHttpClientFactory httpClientFactory, IConfiguration configuration) {
            _nasaApiDbContext = dbContext;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }


        /// <summary>
        /// This is the main method that gets the picture response for the date
        ///1. Validate the query param i.e.date
        ///2. Get response from cache
        ///3. If cache doesn't have then call API.
        ///4. Update cache
        ///5. send back reply
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<ResponseDTO> GetPicOfTheDay(string date)
        {

            string formattedDate = string.IsNullOrEmpty(date)? "" : ValidateAndFormatDateParam(date);
            var returnValue = new ResponseDTO();
            // check the cache contains the response from the data requested
            var cachedDataModel = await GetCachedResponseFromMemoryDbAsync(formattedDate);

            //if cache has data then sent cached data back to the client
            if (cachedDataModel != null)
            {
                // indicate that this is a cached response, mostly for testing and debugging purposes
                returnValue.Message = "CachedResponse";
                returnValue.dataModel = cachedDataModel;
                return returnValue;
            }

            // so cache doesn't have the data then fetch from the Nasa API
            else
            {
                try
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var url = BuildUrl(formattedDate);

                    var response = await httpClient.GetAsync(url);
                    string responseString = await response.Content.ReadAsStringAsync();

                    // if response is a http 200 success, then construct Picture DTO 
                    if (response.IsSuccessStatusCode)
                    {
                        // deserialize JSON into model
                        var data = JsonConvert.DeserializeObject<NasaAstroPicModel>(responseString);
                        returnValue.Message = "Success";
                        returnValue.dataModel = data;

                        // store the response in cache
                        await AddResponseToCache(data);

                        return returnValue;

                    }
                    else
                    {
                        throw new NasaApiResponseException(response.StatusCode,
                            "Error response from Nasa Api, " + responseString);
                    }
                }
                // checked exceptions coming from HTTP client, these are generalized to internal server errors
                catch (HttpRequestException ex)
                {
                    throw new NasaApiResponseException(ex.StatusCode ?? HttpStatusCode.InternalServerError,
                        "Error response from Nasa Api, " + ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    throw new NasaApiResponseException(HttpStatusCode.InternalServerError,
                        "Error response from Nasa Api, " + ex.Message);
                }
                catch (TaskCanceledException ex)
                {
                    throw new NasaApiResponseException(HttpStatusCode.InternalServerError,
                        "Error response from Nasa Api, " + ex.Message);
                }

            }
            

        }

        /// <summary>
        /// Validate query parameter
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string ValidateAndFormatDateParam(string date)
        {
            DateTime formattedDateTime;
            if (DateTime.TryParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out formattedDateTime))
            {
                return formattedDateTime.ToString("yyyy-MM-dd");
            }
            else
            {

                throw new NasaApiResponseException(HttpStatusCode.BadRequest,
                    "Date format is not correct, it should be in yyyy-MM-dd format");
            }
        }

        /// <summary>
        /// Add response to cache to be used for subsequent calls
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task AddResponseToCache(NasaAstroPicModel data)
        {
            _nasaApiDbContext.NasaAstroPicModel.Add(data);
            await _nasaApiDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Check if the response is already cached
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private async Task<NasaAstroPicModel> GetCachedResponseFromMemoryDbAsync(string date) 
        {
            // if date is empty then check for today's date in the cache
            if (string.IsNullOrEmpty(date))
            {
                date = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var cacheData =  await _nasaApiDbContext.NasaAstroPicModel.Where(x => x.Date == date).ToListAsync();
            if (cacheData.Count == 0)
                return null;

            return cacheData[0];
        }

        /// <summary>
        /// URL builder
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string BuildUrl(string date)
        {
            string nasaUri = _configuration["NasaUrl"];
            if (!string.IsNullOrEmpty(date))
            {
                nasaUri += "&date=" + date;
            }

            return nasaUri;
        }
    }
}
