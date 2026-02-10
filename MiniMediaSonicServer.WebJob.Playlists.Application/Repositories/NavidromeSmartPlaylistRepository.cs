using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using Npgsql;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Repositories;

public class NavidromeSmartPlaylistRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    
    public NavidromeSmartPlaylistRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }

    public async Task<List<Guid>> GetTrackIdsForPlaylistAsync(
	    Guid userId,
        string filter, 
        Dictionary<string, object> parameters,
        string sort, 
        string order, 
        int limit)
    {
	    parameters["userId"] = userId;
        parameters["sort"] = sort;
        parameters["order"] = order;
        parameters["limit"] = limit;
        
        string tableSample = sort == "random" ? "TABLESAMPLE SYSTEM (1)" : string.Empty;
        string ordering = !string.IsNullOrWhiteSpace(sort) && sort != "random" ? 
	        $"order by @sort {order}" : string.Empty;
        
        string query = @$"WITH track_playhistory AS (
						     SELECT
						         m.MetadataId,
						         COUNT(*) AS TrackPlaycount,
						         MAX(playhistory.UpdatedAt) AS UpdatedAt
						     FROM sonicserver_user_playhistory playhistory
						     JOIN metadata m
						         ON m.MetadataId = playhistory.TrackId
						     WHERE playhistory.UserId = @userId
						     GROUP BY m.MetadataId
						 )
						 SELECT m.MetadataId
						 from (
	 						select *
	 						from metadata
						    {tableSample}
						 ) m
						 join albums al on al.AlbumId = m.AlbumId
						 join artists a on a.artistid = al.artistid 

						 left join sonicserver_track_rated rated on rated.UserId = @userId and rated.TrackId = m.MetadataId
						 left join sonicserver_artist_rated artist_rated on artist_rated.UserId = @userId and artist_rated.ArtistId = a.ArtistId
						 left join sonicserver_album_rated album_rated on album_rated.UserId = @userId and album_rated.AlbumId = al.AlbumId
						 left join sonicserver_user_playhistory history on history.UserId = @userId and history.TrackId = m.MetadataId
 						 LEFT JOIN LATERAL (
						    SELECT jsonb_object_agg(lower(key), value) AS tags
						    FROM jsonb_each_text(m.tag_alljsontags)
						  ) t ON TRUE
						 LEFT JOIN track_playhistory playhistory
							ON playhistory.MetadataId = m.MetadataId
						 
						 where {filter}
						 {ordering}
						 limit @limit";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();

        return (await conn
                .QueryAsync<Guid>(query, param: parameters))
                .ToList();
    }
}