using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasaApi.Models.DTOs
{
    /// <summary>
    /// This is the DTO that is sent out while requesting from this service.
    /// </summary>
    public class ResponseDTO
    {
        public string Message { get; set; }
        public NasaAstroPicModel dataModel { get; set; }
    }
}
