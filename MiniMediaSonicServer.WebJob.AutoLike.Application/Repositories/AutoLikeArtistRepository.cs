using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using Npgsql;

namespace MiniMediaSonicServer.WebJob.AutoLike.Application.Repositories;

public class AutoLikeArtistRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public AutoLikeArtistRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }

    public async Task<List<Guid>> GetArtistIdToLikeAsync(
	    Guid userId,
        int daysRecent,
	    int listenCount)
    {
        string query = @$"SELECT distinct ar.ArtistId
						  FROM sonicserver_user_playhistory playhistory
						  JOIN metadata m ON m.MetadataId = playhistory.TrackId
						  join albums a on a.albumid = m.albumid 
						  join artists ar on ar.artistid = a.artistid
						  left join sonicserver_artist_rated sar on 
 					  		sar.userid = @userId
 					  		and sar.artistid = ar.artistid 
						  
						  WHERE 
					  		playhistory.UserId = @userId
					  		and playhistory.UpdatedAt > current_timestamp - (@daysRecent * interval '1 day')
					  		
					  		and (sar.ArtistId is null or sar.Starred = false)
 					  		and sar.UnStarredAt is null
 					  	
						  GROUP BY ar.ArtistId
						  having COUNT(*) > @listenCount";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();

        return (await conn
                .QueryAsync<Guid>(query, param: new
                {
	                userId,
	                daysRecent,
	                listenCount
                }))
                .ToList();
    }
}