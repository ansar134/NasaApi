using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NasaApi.Exceptions;
using NasaApi.Models;
using NasaApi.Models.DTOs;
using NasaApi.Services;
using NasaApi.Storage;
using Newtonsoft.Json;

namespace NasaApi.Controllers
{
    /// <summary>
    /// This is the main controller class that gets the request and forwards them to the service
    /// to get the response back from Nasa API
    /// This is from LH branch
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NasaAstroPicController : ControllerBase
    {
        private readonly INasaPicFetchingService _nasaPicFetchingService;
        private readonly ILogger _logger;


        public NasaAstroPicController(INasaPicFetchingService nasaPicFetchingService, ILogger<NasaAstroPicController> logger)
        {
            _nasaPicFetchingService = nasaPicFetchingService;
            _logger = logger;
        }

        // GET: api/NasaAstroPic
        [HttpGet]
        public async Task<IActionResult> GetNasaAstroPicModel(string date)
        {
            _logger.Log(LogLevel.Information,"Getting the NASA image, query param: ", date);
            try
            {
                var response = await _nasaPicFetchingService.GetPicOfTheDay(date);
                _logger.Log(LogLevel.Information, "got the response, sending it back: ", response);
                return Ok(response);
            }
            catch (NasaApiResponseException exception)
            {
                // Add logging
                _logger.Log(LogLevel.Error, "Got exception while getting the response: ",exception);
                return BadRequest(exception.ToJsonString());
            }

            

        }


    }
}
