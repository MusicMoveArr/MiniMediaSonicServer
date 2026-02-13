using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class UserPlayHistoryRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    
    public UserPlayHistoryRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task CreatePlayHistoryAsync(
	    Guid userId, 
	    Guid trackId, 
	    bool scrobble, 
	    DateTime? scrobbleAt, 
	    long playOffset, 
	    DateTime currentDateTime,
	    string importedBy = null)
    {
	    string query = @"INSERT INTO sonicserver_user_playhistory (HistoryId, UserId, TrackId, Scrobble, ScrobbleAt, 
                                          						   Artist, AlbumArtist, Artists, Album, Title, ISRC, 
                                          						   CreatedAt, UpdatedAt, ImportedBy, playOffset)
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
						 	@currentDateTime,
						 	@currentDateTime,
						 	@importedBy,
						 	@playOffset
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
			    scrobbleAt,
			    currentDateTime,
			    importedBy,
			    playOffset
		    });
    }
    
    public async Task<UserPlayHistoryModel?> GetLastUserPlayByTrackIdAsync(Guid userId, Guid trackId, DateTime timeFilter)
    {
	    string query = @"SELECT * FROM sonicserver_user_playhistory
					     WHERE UserId = @userId
					     AND TrackId = @trackId
					     AND CreatedAt >= @timeFilter
					     order by UpdatedAt desc
						 limit 1";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return await conn.QueryFirstOrDefaultAsync<UserPlayHistoryModel>(query, 
		    param: new
		    {
			    userId,
			    trackId,
			    timeFilter
		    });
    }
    
    public async Task UpdateUserPlayHistoryAsync(Guid historyId, bool scrobble, DateTime? scrobbleAt, DateTime currentDateTime)
    {
	    string query = @"UPDATE sonicserver_user_playhistory 
						 SET Scrobble = @scrobble,
						     ScrobbleAt = @scrobbleAt,
						     UpdatedAt = @currentDateTime						 
						 where HistoryId = @historyId";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    historyId,
			    scrobble,
			    scrobbleAt,
			    currentDateTime
		    });
    }
    
    public async Task<List<UserPlayHistoryModel>> GetScrobbledAtTimeAsync(
	    Guid userId, 
	    DateTime timeFilter,
	    long epochPlayOffset)
    {
	    string query = @"SELECT * 
						 FROM sonicserver_user_playhistory
					     WHERE UserId = @userId
					     AND (CreatedAt between @timeFilterFrom and @timeFilterTill
					     	 or ScrobbleAt between @timeFilterFrom and @timeFilterTill
					     	 or PlayOffset between @epochPlayOffset-5 and @epochPlayOffset+5)";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return (await conn.QueryAsync<UserPlayHistoryModel>(query,
		    param: new
		    {
			    userId,
			    timeFilterFrom = timeFilter.Subtract(TimeSpan.FromSeconds(5)),
			    timeFilterTill = timeFilter.Add(TimeSpan.FromSeconds(5)),
			    epochPlayOffset
		    }))
		    .ToList();
    }
}