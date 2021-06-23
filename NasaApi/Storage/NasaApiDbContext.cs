using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasaApi.Models;

namespace NasaApi.Storage
{
    /// <summary>
    /// A very simple version of in-memory DB that is caching responses
    /// </summary>
    public class NasaApiDbContext: DbContext
    {
        public NasaApiDbContext(DbContextOptions<NasaApiDbContext> options)
            : base(options)
        {

        }
        public DbSet<NasaAstroPicModel> NasaAstroPicModel { get; set; }


    }
}
