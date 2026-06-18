using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using Npgsql;

namespace MiniMediaSonicServer.WebJob.Indexing.Application.Repositories;

public class FixMissingRatedTracksRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;

    public FixMissingRatedTracksRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }

    public async Task<List<RatedTrackModel>> GetMissingRatedTracksAsync()
    {
        string query = @"select distinct on (track.TrackId) track.*
                         from sonicserver_track_rated track
                         left join metadata m on m.metadataid = track.trackid 
                         where m.metadataid is null";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();

        return (await conn
                .QueryAsync<RatedTrackModel>(query))
            .ToList();
    }
    
    public async Task ReplaceMissingRatedTrackAsync(
        Guid oldTrackId,
        Guid newTrackId,
        string artist,
        string albumArtist,
        string artists,
        string album,
        string title,
        string isrc)
    {
        string query = @"UPDATE sonicserver_track_rated t
                         SET TrackId = @newTrackId,
                             Artist = COALESCE(NULLIF(@artist, ''), t.Artist),
                             AlbumArtist = COALESCE(NULLIF(@albumArtist, ''), t.AlbumArtist),
                             Artists = COALESCE(NULLIF(@artists, ''), t.Artists),
                             Album = COALESCE(NULLIF(@album, ''), t.Album),
                             Title = COALESCE(NULLIF(@title, ''), t.Title),
                             Isrc = COALESCE(NULLIF(@isrc, ''), t.Isrc)
                         where TrackId = @oldTrackId
                         AND NOT EXISTS (
                             SELECT 1 FROM sonicserver_track_rated rated WHERE rated.TrackId = @newTrackId and rated.UserId = t.UserId
                         )";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();

        await conn.ExecuteAsync(query, param: new
        {
            oldTrackId,
            newTrackId,
            artist,
            albumArtist,
            artists,
            album,
            title,
            isrc
        });
    }
}