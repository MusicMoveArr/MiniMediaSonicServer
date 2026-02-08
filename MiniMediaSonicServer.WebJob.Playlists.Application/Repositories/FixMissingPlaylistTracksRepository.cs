using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using Npgsql;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Repositories;

public class FixMissingPlaylistTracksRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    
    public FixMissingPlaylistTracksRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }

    public async Task<List<PlaylistTrackModel>> GetMissingPlaylistTracksAsync()
    {
        string query = @"select distinct on(track.TrackId) track.*
                         from sonicserver_playlist_track track
                         left join metadata m on m.metadataid = track.trackid 
                         where m.metadataid is null";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();

        return (await conn
                .QueryAsync<PlaylistTrackModel>(query))
                .ToList();
    }

    public async Task ReplaceMissingPlaylistTrackAsync(
        Guid oldTrackId, 
        Guid newTrackId,
        string artist,
        string albumArtist,
        string artists,
        string album,
        string title,
        string isrc)
    {
        string query = @"UPDATE sonicserver_playlist_track t
                         SET TrackId = @newTrackId,
                             Artist = @artist,
                             AlbumArtist = @albumArtist,
                             Artists = @artists,
                             Album = @album,
                             Title = @title,
                             Isrc = @isrc
                         where TrackId = @oldTrackId
                            and not exists(
                                select 1 from sonicserver_playlist_track track
                                where track.PlayListId = t.PlayListId and track.TrackId = @newTrackId
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