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
    
    [Theory]
    [InlineData("newest")]
    [InlineData("recent")]
    [InlineData("frequent")]
    [InlineData("random")]
    [InlineData("starred")]
    [InlineData("alphabeticalByName")]
    public async Task Benchmark_GetAlbum(string type)
    {
        var request = GetRequest("/rest/getAlbumList2");
        request.AddParameter("type", type);
        
        await BenchmarkTest.BenchmarkTestAsync(150, 100, async () =>
        {
            var response = await Client.GetAsync<SubsonicEnvelope>(request);
            response.Response.AlbumList2.Album.Count.Should().BeGreaterThan(1);
            response.Response.Status.Should().Be("ok");
        });
    }
}
