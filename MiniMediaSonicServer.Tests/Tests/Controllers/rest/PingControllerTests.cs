using AwesomeAssertions;
using BaseTests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using BaseTests.Attributes;
using RestSharp;

namespace MiniMediaSonicServer.Tests.Tests.Controllers.rest;

public class PingControllerTests : IntegrationTest
{
    public PingControllerTests(ApiWebApplicationFactory fixture)
        : base(fixture)
    {
        
    }
    
    [Fact]
    //[Repeat(1000)]
    public async Task GET_retrieves_ping_forecast()
    {
        var response = await Client.GetAsync<SubsonicResponse>(GetRequest("/rest/ping"), TestContext.Current.CancellationToken);
        response?.Status.Should().Be("ok");
    }
}
