using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NasaApi.Controllers;
using NasaApi.Models.DTOs;
using NasaApi.Services.Impl;
using NasaApi.Storage;
using NasaApiTest.Util;
using Xunit;

namespace NasaApiTest.Controllers
{
    public class NasaAstroPicControllerTest
    {
        [Fact]
        public async Task ReturnsOkResponseWithApiData()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<NasaApiDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbNasaApiPictures");
            var dbContext = new NasaApiDbContext(optionsBuilder.Options);
            var clientFactory = HttpClientMock.NasaApiHTTPClientFactory(NasaApiResponseBuilder.OkResponse, HttpStatusCode.OK);

            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var service = new NasaPicFetchingService(dbContext, clientFactory, config);
            var systemUnderTest = new NasaAstroPicController(service, new NullLogger<NasaAstroPicController>());
            
            //act
            var result = await systemUnderTest.GetNasaAstroPicModel("") as OkObjectResult;

            //assert
            Assert.IsType<ResponseDTO> (result.Value);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task ReturnsOkResponseWithCachedApiData()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<NasaApiDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbNasaApiPictures");
            var dbContext = new NasaApiDbContext(optionsBuilder.Options);
            var clientFactory = HttpClientMock.NasaApiHTTPClientFactory(NasaApiResponseBuilder.OkResponse, HttpStatusCode.OK);
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var service = new NasaPicFetchingService(dbContext, clientFactory, config);
            var systemUnderTest = new NasaAstroPicController(service, new NullLogger<NasaAstroPicController>());

            //act
            var result = await systemUnderTest.GetNasaAstroPicModel("") as OkObjectResult;
            
            //assert
            Assert.NotNull(result.Value);
            Assert.IsType<ResponseDTO>(result.Value);
            result = await systemUnderTest.GetNasaAstroPicModel("") as OkObjectResult;
            Assert.NotNull(result.Value);
            Assert.IsType<ResponseDTO>(result.Value);
            Assert.Equal("CachedResponse", (result.Value as ResponseDTO).Message);
        }

        [Fact]
        public async Task ReturnsBadRequestWhenDateQueryParamIsInFuture()
        {
            //arrange
            var optionsBuilder = new DbContextOptionsBuilder<NasaApiDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbNasaApiPictures");
            var dbContext = new NasaApiDbContext(optionsBuilder.Options);
            var clientFactory = HttpClientMock.NasaApiHTTPClientFactory(NasaApiResponseBuilder.BadRequestResponse,HttpStatusCode.BadRequest);
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var service = new NasaPicFetchingService(dbContext, clientFactory, config);
            var systemUnderTest = new NasaAstroPicController(service, new NullLogger<NasaAstroPicController>());
            
            //act
            var result = await systemUnderTest.GetNasaAstroPicModel(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")) as ObjectResult;

            //assert
            Assert.Equal(400,result.StatusCode);

        }
    }
}
