using AwesomeAssertions;
using BaseTests;
using BaseTests.Helpers;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using RestSharp;

namespace MiniMediaSonicServer.Tests.Tests.Controllers.rest;

public class GetAlbumList2ControllerTests : IntegrationTest
{
    public GetAlbumList2ControllerTests(ApiWebApplicationFactory fixture)
        : base(fixture)
    {
        
    }
    
    [Fact]
    public async Task Benchmark_GetAlbum_Newest()
    {
        var request = GetRequest("/rest/getAlbumList2");
        request.AddParameter("type", "newest");
        
        await BenchmarkTest.BenchmarkTestAsync(150, 100, async () =>
        {
            var response = await Client.GetAsync<SubsonicEnvelope>(request);
            response.Response.AlbumList2.Album.Count.Should().BeGreaterThan(1);
            response.Response.Status.Should().Be("ok");
        });
    }
}
