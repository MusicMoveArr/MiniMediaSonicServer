using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class UserPlayHistoryRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    
    public UserPlayHistoryRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task CreatePlayHistoryAsync(Guid userId, Guid trackId, bool scrobble, DateTime? scrobbleAt)
    {
	    string query = @"INSERT INTO sonicserver_user_playhistory (HistoryId, UserId, TrackId, Scrobble, ScrobbleAt, 
                                          						   Artist, AlbumArtist, Artists, Album, Title, ISRC, CreatedAt, UpdatedAt)
						 SELECT
						    @historyId,
							@userId,
							@trackId,
							@scrobble,
							@scrobbleAt,
						    COALESCE(t.tags->>'artist', ''),
						    COALESCE(COALESCE(t.tags->>'albumartist', t.tags->>'album_artist'), ''),
						    COALESCE(t.tags->>'artists', ''),
						    COALESCE(t.tags->>'album', ''),
						    COALESCE(m.Title, ''),
						    COALESCE(t.tags->>'isrc', ''),
						 	current_timestamp,
						 	current_timestamp
						 FROM metadata m
						 LEFT JOIN LATERAL (
						    SELECT jsonb_object_agg(lower(key), value) AS tags
						    FROM jsonb_each_text(m.tag_alljsontags)
						  ) t ON TRUE
						 WHERE m.MetadataId = @trackId";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    historyId = Guid.NewGuid(),
			    userId,
			    trackId,
			    scrobble,
			    scrobbleAt
		    });
    }
    
    public async Task<UserPlayHistoryModel?> GetLastUserPlayByTrackIdAsync(Guid userId, Guid trackId)
    {
	    string query = @"SELECT * FROM sonicserver_user_playhistory
					     WHERE UserId = @userId
					     AND TrackId = @trackId
					     order by UpdatedAt desc
						 limit 1";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return await conn.QueryFirstOrDefaultAsync<UserPlayHistoryModel>(query, 
		    param: new
		    {
			    userId,
			    trackId
		    });
    }
    
    public async Task UpdateUserPlayHistoryAsync(Guid historyId, bool scrobble, DateTime? scrobbleAt)
    {
	    string query = @"UPDATE sonicserver_user_playhistory 
						 SET Scrobble = @scrobble,
						     ScrobbleAt = @scrobbleAt,
						     UpdatedAt = current_timestamp						 
						 where HistoryId = @historyId";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    historyId,
			    scrobble,
			    scrobbleAt
		    });
    }
}