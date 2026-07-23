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
            response?.Response?.Album?.Id.Should().Be(album.Id);
            response?.Response?.Status.Should().Be("ok");
        });
    }
    
    [Fact]
    public async Task Benchmark_GetAlbums()
    {
        var albums = await RandomDataHelper.GetRandomAlbumsAsync(this, 101);

        albums.Should().NotBeNull();
        
        var albumIds = albums.Select(a => a.Id).ToList();
        int offset = 0;
        
        await BenchmarkTest.BenchmarkTestAsync(70, 100, async () =>
        {
            Guid albumId = albumIds[offset++];
            
            var request = GetRequest("/rest/getAlbum");
            request.AddParameter("id", albumId);
        
            var response = await Client.GetAsync<SubsonicEnvelope>(request);
            response?.Response?.Album?.Id.Should().Be(albumId);
            response?.Response?.Status.Should().Be("ok");
        });
    }
}
