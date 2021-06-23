using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace NasaApiTest.Util
{
    public static class HttpClientMock
    {
        public static IHttpClientFactory NasaApiHTTPClientFactory(StringContent content, HttpStatusCode statusCode)
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = content
                });
            var client = new HttpClient(handler.Object);
            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
                .Returns(client);
            return clientFactory.Object;
        }

        public static IHttpClientFactory NasaApiHTTPClientExceptionFactory(HttpContent content, HttpStatusCode statusCode)
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = content
                });
            var client = new HttpClient(handler.Object);
            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
                .Returns(client);
            return clientFactory.Object;
        }

        //Class to mock exception coming from HTTP client
        public class ExceptionContent : HttpContent
        {
            private readonly Exception exception;
            public ExceptionContent(Exception exception)
            {
                this.exception = exception;
            }
            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                return Task.FromException(exception);
            }

            protected override bool TryComputeLength(out long length)
            {
                length = 0L;
                return false;
            }

        }
    }
}
