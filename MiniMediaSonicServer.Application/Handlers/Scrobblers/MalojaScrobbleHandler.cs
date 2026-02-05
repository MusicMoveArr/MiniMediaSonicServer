using MiniMediaSonicServer.Application.Interfaces;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.Maloja;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using RestSharp;

namespace MiniMediaSonicServer.Application.Handlers.Scrobblers;

public class MalojaScrobbleHandler : IScrobble
{
    public async Task ScrobbleAsync(TrackID3 track, UserModel user, DateTime scrobbleAt)
    {
        int fourMinutes = (int)TimeSpan.FromMinutes(4).TotalSeconds;
        int scrobbleFor = fourMinutes > track.Duration ? fourMinutes :  (int)(track.Duration * 0.8F);
        
        var scrobbleModel = new ScrobbleTrackEntity
        {
            Artists = [track.Artist],
            Albumartists = track.AlbumArtists
                ?.Select(a => a.Name)
                ?.Distinct()
                ?.ToList() ?? [],
            Title = track.Title,
            Album = track.Album,
            Duration = scrobbleFor,
            Length = track.Duration,
            Key = user.MalojaApiKey
        };
        
        RestClientOptions options = new RestClientOptions(user.MalojaUrl);
        RestClient client = new RestClient(options);
        RestRequest request = new RestRequest("newscrobble", Method.Post);
        request.AddJsonBody(scrobbleModel);
        await client.ExecuteAsync(request);
    }
}