using AwesomeAssertions;
using BaseTests;
using BaseTests.Helpers;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using RestSharp;

namespace MiniMediaSonicServer.Tests.Tests.Controllers.rest;

public class Search3ControllerTests : IntegrationTest
{
    public Search3ControllerTests(ApiWebApplicationFactory fixture)
        : base(fixture)
    {
        
    }
    
    [Fact]
    public async Task LastIsFirstBug_Artists()
    {
        //first half
        var request = GetRequest("/rest/search3");
        request.AddParameter("ArtistCount", 50);
        request.AddParameter("ArtistOffset", 0);
        request.AddParameter("AlbumCount", 0);
        request.AddParameter("AlbumOffset", 0);
        request.AddParameter("SongCount", 0);
        request.AddParameter("songOffset", 0);
        var firstResponse = await Client.GetAsync<SubsonicEnvelope>(request);
        
        //the next 50 
        request = GetRequest("/rest/search3");
        request.AddParameter("ArtistCount", 50);
        request.AddParameter("ArtistOffset", 50);
        request.AddParameter("AlbumCount", 0);
        request.AddParameter("AlbumOffset", 0);
        request.AddParameter("SongCount", 0);
        request.AddParameter("songOffset", 0);
        var secondResponse = await Client.GetAsync<SubsonicEnvelope>(request);

        firstResponse.Response.Status.Should().Be("ok");
        secondResponse.Response.Status.Should().Be("ok");
        
        firstResponse.Response.SearchResult3.Artists.Count.Should().Be(50);
        secondResponse.Response.SearchResult3.Artists.Count.Should().Be(50);

        Guid firstId = firstResponse.Response.SearchResult3.Artists.First().Id;
        secondResponse.Response.SearchResult3.Artists.First().Id.Should().NotBe(firstId);
    }
    
    [Fact]
    public async Task LastIsFirstBug_Albums()
    {
        //first half
        var request = GetRequest("/rest/search3");
        request.AddParameter("ArtistCount", 0);
        request.AddParameter("ArtistOffset", 0);
        request.AddParameter("AlbumCount", 50);
        request.AddParameter("AlbumOffset", 0);
        request.AddParameter("SongCount", 0);
        request.AddParameter("songOffset", 0);
        var firstResponse = await Client.GetAsync<SubsonicEnvelope>(request);
        
        //the next 50 
        request = GetRequest("/rest/search3");
        request.AddParameter("ArtistCount", 0);
        request.AddParameter("ArtistOffset", 0);
        request.AddParameter("AlbumCount", 50);
        request.AddParameter("AlbumOffset", 50);
        request.AddParameter("SongCount", 0);
        request.AddParameter("songOffset", 0);
        var secondResponse = await Client.GetAsync<SubsonicEnvelope>(request);

        firstResponse.Response.Status.Should().Be("ok");
        secondResponse.Response.Status.Should().Be("ok");
        
        firstResponse.Response.SearchResult3.Albums.Count.Should().Be(50);
        secondResponse.Response.SearchResult3.Albums.Count.Should().Be(50);

        Guid firstId = firstResponse.Response.SearchResult3.Albums.First().Id;
        secondResponse.Response.SearchResult3.Albums.First().Id.Should().NotBe(firstId);
    }
    
    [Fact]
    public async Task LastIsFirstBug_Tracks()
    {
        //first half
        var request = GetRequest("/rest/search3");
        request.AddParameter("ArtistCount", 0);
        request.AddParameter("ArtistOffset", 0);
        request.AddParameter("AlbumCount", 0);
        request.AddParameter("AlbumOffset", 0);
        request.AddParameter("SongCount", 50);
        request.AddParameter("songOffset", 0);
        var firstResponse = await Client.GetAsync<SubsonicEnvelope>(request);
        
        //the next 50 
        request = GetRequest("/rest/search3");
        request.AddParameter("ArtistCount", 0);
        request.AddParameter("ArtistOffset", 0);
        request.AddParameter("AlbumCount", 0);
        request.AddParameter("AlbumOffset", 0);
        request.AddParameter("SongCount", 50);
        request.AddParameter("songOffset", 50);
        var secondResponse = await Client.GetAsync<SubsonicEnvelope>(request);

        firstResponse.Response.Status.Should().Be("ok");
        secondResponse.Response.Status.Should().Be("ok");
        
        firstResponse.Response.SearchResult3.Tracks.Count.Should().Be(50);
        secondResponse.Response.SearchResult3.Tracks.Count.Should().Be(50);

        Guid firstId = firstResponse.Response.SearchResult3.Tracks.First().TrackId;
        secondResponse.Response.SearchResult3.Tracks.First().TrackId.Should().NotBe(firstId);
    }
    
    [Theory]
    [InlineData(50, 0)]
    [InlineData(50, 50)]
    [InlineData(50, 49)]
    [InlineData(0, 50)]
    public async Task SearchCountBug_Artists(int count, int offset)
    {
        var request = GetRequest("/rest/search3");
        request.AddParameter("ArtistCount", count);
        request.AddParameter("ArtistOffset", offset);
        request.AddParameter("AlbumCount", 0);
        request.AddParameter("AlbumOffset", 0);
        request.AddParameter("SongCount", 0);
        request.AddParameter("songOffset", 0);
        var response = await Client.GetAsync<SubsonicEnvelope>(request);
        response.Response.Status.Should().Be("ok");
        response.Response.SearchResult3.Artists.Count.Should().Be(count);
    }
    
    [Theory]
    [InlineData(50, 0)]
    [InlineData(50, 50)]
    [InlineData(50, 49)]
    [InlineData(0, 50)]
    public async Task SearchCountBug_Albums(int count, int offset)
    {
        var request = GetRequest("/rest/search3");
        request.AddParameter("ArtistCount", 0);
        request.AddParameter("ArtistOffset", 0);
        request.AddParameter("AlbumCount", count);
        request.AddParameter("AlbumOffset", offset);
        request.AddParameter("SongCount", 0);
        request.AddParameter("songOffset", 0);
        var response = await Client.GetAsync<SubsonicEnvelope>(request);
        response.Response.Status.Should().Be("ok");
        response.Response.SearchResult3.Albums.Count.Should().Be(count);
    }
    
    [Theory]
    [InlineData(50, 0)]
    [InlineData(50, 50)]
    [InlineData(50, 49)]
    [InlineData(0, 50)]
    public async Task SearchCountBug_Tracks(int count, int offset)
    {
        var request = GetRequest("/rest/search3");
        request.AddParameter("ArtistCount", 0);
        request.AddParameter("ArtistOffset", 0);
        request.AddParameter("AlbumCount", 0);
        request.AddParameter("AlbumOffset", 0);
        request.AddParameter("SongCount", count);
        request.AddParameter("songOffset", offset);
        var response = await Client.GetAsync<SubsonicEnvelope>(request);
        response.Response.Status.Should().Be("ok");
        response.Response.SearchResult3.Tracks.Count.Should().Be(count);
    }
    
}
