using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using RestSharp;

namespace BaseTests.Helpers;

public class RandomDataHelper
{
    public static async Task<AlbumID3?> GetRandomAlbumAsync(IntegrationTest integrationTest)
    {
        return (await GetRandomAlbumsAsync(integrationTest, 1))?.FirstOrDefault();
    }
    public static async Task<List<AlbumID3>?> GetRandomAlbumsAsync(IntegrationTest integrationTest, int count)
    {
        var request = integrationTest.GetRequest("/rest/getAlbumList2");
        request.AddParameter("type", "random");
        request.AddParameter("size", count);
        request.AddParameter("offset", "0");

        return (await integrationTest.Client.GetAsync<SubsonicEnvelope>(request))
            ?.Response
            ?.AlbumList2
            ?.Album.ToList();
    }
}