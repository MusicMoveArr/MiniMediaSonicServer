using AwesomeAssertions;
using BaseTests;
using BaseTests.Helpers;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using RestSharp;

namespace MiniMediaSonicServer.Tests.Tests.Controllers.rest;

public class GetAlbumControllerTests : IntegrationTest
{
    public GetAlbumControllerTests(ApiWebApplicationFactory fixture)
        : base(fixture)
    {
        
    }
    
    [Fact]
    public async Task Benchmark_GetAlbum()
    {
        var album = await RandomDataHelper.GetRandomAlbumAsync(this);
        
        var request = GetRequest("/rest/getAlbum");
        request.AddParameter("id", album.Id);
        
        await BenchmarkTest.BenchmarkTestAsync(30, 100, async () =>
        {
            var response = await Client.GetAsync<SubsonicEnvelope>(request);
            response.Response.Album.Id.Should().Be(album.Id);
            response.Response.Status.Should().Be("ok");
        });
    }
}
