using System.Text.Json;
using MiniMediaSonicServer.Application.Interfaces;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.ListenBrainz;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using RestSharp;

namespace MiniMediaSonicServer.Application.Handlers.Scrobblers;

public class ListenBrainzScrobbleHandler : IScrobble
{
    public async Task ScrobbleAsync(TrackID3 track, UserModel user, DateTime scrobbleAt)
    {
        SubmitModel submitModel = new SubmitModel
        {
            ListenType = "single",
            Payload =
            [
                new SubmitPayloadModel
                {
                    ListenedAt = new DateTimeOffset(scrobbleAt).ToUnixTimeSeconds(),
                    TrackMetadata = new SubmitPayloadTrackMetadataModel
                    {
                        AdditionalInfo = new SubmitPayloadTrackMetadataAdditionalInfoModel
                        {
                            ArtistMbId = [],
                            DurationMs = track.Duration * 1000
                        },
                        ArtistName = track.Artist,
                        ReleaseName = track.Album,
                        TrackName = track.Title
                    }
                }
            ]
        };
        
        RestClientOptions options = new RestClientOptions("https://api.listenbrainz.org/1/");
        RestClient client = new RestClient(options);
        RestRequest request = new RestRequest("submit-listens", Method.Post);
        request.AddHeader("Authorization", $"Token {user.ListenBrainzUserToken}");
        request.AddJsonBody(submitModel);
        await client.ExecuteAsync(request);
    }
}