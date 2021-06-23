using NasaApi.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasaApi.Services
{
    /// <summary>
    /// Interface to the service for IoC container to use
    /// </summary>
    public interface INasaPicFetchingService
    {
        Task<ResponseDTO> GetPicOfTheDay(string date);
    }
}
