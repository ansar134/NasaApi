using Microsoft.EntityFrameworkCore;
using NasaApi.Models;
using NasaApi.Services.Impl;
using NasaApi.Storage;
using NasaApiTest.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using NasaApi.Exceptions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace NasaApiTest.Services
{
    public class NasaPicFetchingServiceTest
    {
        [Fact]
        public async Task ReturnNasaImageForTheDay()
        {
            //arrange
            var optionsBuilder = new DbContextOptionsBuilder<NasaApiDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbNasaApiPictures");
            var dbContext = new NasaApiDbContext(optionsBuilder.Options);
            var clientFactory = HttpClientMock.NasaApiHTTPClientFactory(NasaApiResponseBuilder.OkResponse, HttpStatusCode.OK);
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var systemUnderTest = new NasaPicFetchingService(dbContext, clientFactory, config);
            
            //act
            var result = await systemUnderTest.GetPicOfTheDay("");
            
            //assert
            Assert.NotNull(result.dataModel);
            Assert.IsType<NasaAstroPicModel>(result.dataModel);

        }

        [Fact]
        public async Task VerifyTheContentsOfResponse()
        {
            //arrange
            var optionsBuilder = new DbContextOptionsBuilder<NasaApiDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbNasaApiPictures");
            var dbContext = new NasaApiDbContext(optionsBuilder.Options);
            var clientFactory = HttpClientMock.NasaApiHTTPClientFactory(NasaApiResponseBuilder.OkResponse, HttpStatusCode.OK);
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var systemUnderTest = new NasaPicFetchingService(dbContext, clientFactory, config);
            
            //act
            var result = await systemUnderTest.GetPicOfTheDay("");

            //assert
            Assert.NotNull(result.dataModel);
            Assert.IsType<NasaAstroPicModel>(result.dataModel);
            Assert.NotEqual("",result.dataModel.HdUrl);
            Assert.NotEqual("", result.dataModel.Url);
            Assert.NotEqual("", result.dataModel.Title);
            Assert.NotEqual("", result.dataModel.Title);
            Assert.NotEqual("", result.dataModel.Media_Type);

        }


        [Fact]
        public async Task ReturnExceptionWhenDateIsInFuture()
        {
            //arrange
            var optionsBuilder = new DbContextOptionsBuilder<NasaApiDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbNasaApiPictures");
            var dbContext = new NasaApiDbContext(optionsBuilder.Options);
            var clientFactory = HttpClientMock.NasaApiHTTPClientFactory(NasaApiResponseBuilder.BadRequestResponse, HttpStatusCode.BadRequest);
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var systemUnderTest = new NasaPicFetchingService(dbContext, clientFactory, config);
            
            //act
            try
            {
                var result = await systemUnderTest.GetPicOfTheDay(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            }

            //assert
            catch (NasaApiResponseException ex)
            {
                Assert.IsType<NasaApiResponseException>(ex);
                Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
                Assert.NotNull(JToken.Parse(ex.ToJsonString()));
            }

        }
        
        [Fact]
        public async Task ReturnExceptionWhenDateFormatIsWrong()
        {
            //arrange
            var optionsBuilder = new DbContextOptionsBuilder<NasaApiDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbNasaApiPictures");
            var dbContext = new NasaApiDbContext(optionsBuilder.Options);
            var clientFactory = HttpClientMock.NasaApiHTTPClientFactory(NasaApiResponseBuilder.BadRequestResponse, HttpStatusCode.BadRequest);
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var systemUnderTest = new NasaPicFetchingService(dbContext, clientFactory, config);
            
            //act
            try
            {
                var result = await systemUnderTest.GetPicOfTheDay("2021-5-5");
            }

            //assert
            catch (NasaApiResponseException ex)
            {
                Assert.IsType<NasaApiResponseException>(ex);
                Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
                Assert.Equal("Date format is not correct, it should be in yyyy-MM-dd format", ex.Message);

            }

        }

        [Fact]
        public async Task ReturnExceptionWhenApiIsUnreachable()
        {
            //arrange
            var optionsBuilder = new DbContextOptionsBuilder<NasaApiDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbNasaApiPictures");
            var dbContext = new NasaApiDbContext(optionsBuilder.Options);
            var clientFactory = HttpClientMock.NasaApiHTTPClientExceptionFactory(new HttpClientMock.ExceptionContent(new HttpRequestException("Api is not reachable")), HttpStatusCode.InternalServerError);
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var systemUnderTest = new NasaPicFetchingService(dbContext, clientFactory, config);
            
            //act
            try
            {
                var result = await systemUnderTest.GetPicOfTheDay("2021-05-05");
            }

            //assert
            catch (NasaApiResponseException ex)
            {
                Assert.IsType<NasaApiResponseException>(ex);
                Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
                Assert.Equal("Error response from Nasa Api, Api is not reachable", ex.Message);

            }

        }



        [Fact]
        public async Task ReturnInvalidOperationExceptionFromHttpClient()
        {
            //arrange
            var optionsBuilder = new DbContextOptionsBuilder<NasaApiDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbNasaApiPictures");
            var dbContext = new NasaApiDbContext(optionsBuilder.Options);
            var clientFactory = HttpClientMock.NasaApiHTTPClientExceptionFactory(new HttpClientMock.ExceptionContent(new InvalidOperationException("Invalid Operation")), HttpStatusCode.InternalServerError);
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var systemUnderTest = new NasaPicFetchingService(dbContext, clientFactory, config);
            
            //act
            try
            {
                var result = await systemUnderTest.GetPicOfTheDay("2021-05-05");
            }

            //assert
            catch (NasaApiResponseException ex)
            {
                Assert.IsType<NasaApiResponseException>(ex);
                Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
                Assert.Equal("Error response from Nasa Api, Invalid Operation", ex.Message);

            }

        }

        [Fact]
        public async Task ReturnTaskCanceledExceptionFromHttpClient()
        {
            //arrange
            var optionsBuilder = new DbContextOptionsBuilder<NasaApiDbContext>();
            optionsBuilder.UseInMemoryDatabase("dbNasaApiPictures");
            var dbContext = new NasaApiDbContext(optionsBuilder.Options);
            var clientFactory = HttpClientMock.NasaApiHTTPClientExceptionFactory(new HttpClientMock.ExceptionContent(new TaskCanceledException("Task cancelled")), HttpStatusCode.InternalServerError);
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var systemUnderTest = new NasaPicFetchingService(dbContext, clientFactory, config);
            
            //act
            try
            {
                var result = await systemUnderTest.GetPicOfTheDay("2021-05-05");
            }

            //assert
            catch (NasaApiResponseException ex)
            {
                Assert.IsType<NasaApiResponseException>(ex);
                Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
                Assert.Equal("Error response from Nasa Api, Task cancelled", ex.Message);

            }

        }

    }
}
