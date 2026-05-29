using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using RestSharp;

namespace MiniMediaSonicServer.Tests.Helpers;

public class RandomDataHelper
{
    public static async Task<AlbumID3?> GetRandomAlbumAsync(IntegrationTest integrationTest)
    {
        var request = integrationTest.GetRequest("/rest/getAlbumList2");
        request.AddParameter("type", "random");
        request.AddParameter("size", "1");
        request.AddParameter("offset", "0");

        return (await integrationTest.Client.GetAsync<SubsonicEnvelope>(request))
            ?.Response
            ?.AlbumList2
            ?.Album
            ?.FirstOrDefault();
    }
}