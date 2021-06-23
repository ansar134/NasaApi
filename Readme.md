# Nasa API

This microservice is made using:
- .Net Core 5.0
- EF Core memory for cached reponses
- XUnit for testing
- Swagger for API documentation
- Docker support i.e. docker file to push service to Linux container
- Standard .Net Core IoC is used, for more complex projects Autofac is even better.

## Flow of the service
The service has a REST interface where either a front-end or backend service can request it to get picture of the day from Nasa APOD API.
The GET method accepts an optional query parameter as date and the format should be "yyyy-MM-dd". Exception is thrown if the format is not right.

## Caching of the responses
If a response (identified by the date) is already fetched then its stored in a EF core based memory database. Subsequent queries are fetched from cache instead from APOD API. This mechanism can be further improved by:
- Using Redis or distributed Redis as cache as opposed to using EF core.
- There should be some cache expiring mechanism after which cache is updated.

## How to run the service
The service is able to run from VS Studio in either IIS express or in docker container. The run confgurations are already there.

## Test Project
Test project is added and testing is done using XUnit framework. Code coverage report is also added separately in HTML format. Following improvement can be further made:
- Include Startup and Program classes into the test through integration tests.
- Add more test cases
