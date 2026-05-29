using AwesomeAssertions;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using RestSharp;

namespace MiniMediaSonicServer.Tests.Tests.Controllers.rest;

public class PingControllerTests : IntegrationTest
{
    public PingControllerTests(ApiWebApplicationFactory fixture)
        : base(fixture)
    {
        
    }
    
    [Fact]
    public async Task GET_retrieves_ping_forecast()
    {
        var response = await Client.GetAsync<SubsonicResponse>(GetRequest("/rest/ping"));
        response.Status.Should().Be("ok");
    }
}
