using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using Npgsql;

namespace MiniMediaSonicServer.WebJob.Indexing.Application.Repositories;

public class FixMissingPlayHistoryTracksRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    
    public FixMissingPlayHistoryTracksRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }

    public async Task<List<UserPlayHistoryModel>> GetMissingPlayHistoryTracksAsync()
    {
        string query = @"select hist.*
                         from sonicserver_user_playhistory hist
                         left join metadata m on m.metadataid = hist.trackid 
                         where m.metadataid is null";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();

        return (await conn
                .QueryAsync<UserPlayHistoryModel>(query))
                .ToList();
    }

    public async Task ReplaceMissingPlayHistoryAsync(
        Guid historyId,
        Guid oldTrackId, 
        Guid newTrackId,
        string artist,
        string albumArtist,
        string artists,
        string album,
        string title,
        string isrc)
    {
        string query = @"UPDATE sonicserver_user_playhistory t
                         SET TrackId = @newTrackId,
                             Artist = @artist,
                             AlbumArtist = @albumArtist,
                             Artists = @artists,
                             Album = @album,
                             Title = @title,
                             Isrc = @isrc
                         where HistoryId = @historyId
                            and TrackId = @oldTrackId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();

        await conn.ExecuteAsync(query, param: new
        {
            historyId,
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