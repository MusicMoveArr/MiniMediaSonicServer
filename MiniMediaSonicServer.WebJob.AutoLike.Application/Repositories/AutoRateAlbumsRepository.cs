using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Models.Database;
using Npgsql;

namespace MiniMediaSonicServer.WebJob.AutoLike.Application.Repositories;

public class AutoRateAlbumsRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public AutoRateAlbumsRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<List<AlbumRatingModel>> GetAlbumsToRateAsync(
	    Guid userId,
	    int minimumRating,
	    int trackRatingPercentage)
    {
	    string query = @$"SELECT 
							  al.AlbumId, 
							  avg_rating.AvgRating::int
						  FROM albums al
						  join lateral (
							  select 
								  avg(rated.Rating) as AvgRating, 
								  (count(rated.TrackId)::float / count(m.MetadataId)::float) * 100 as TracksPercentage
							  from metadata m
							  left join sonicserver_track_rated rated on rated.TrackId = m.MetadataId
							  where m.AlbumId = al.AlbumId
						  ) avg_rating on 1=1
						  where
						  not exists (
							  select 1 
							  from sonicserver_album_rated rated
							  where 
								  rated.albumid = al.albumid 
								  and rated.userid = @userId
							  	  and rated.Rating >= 1)
						  and avg_rating.AvgRating >= @minimumRating
						  and avg_rating.TracksPercentage >= @trackRatingPercentage";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.OpenAsync();

	    return (await conn
			    .QueryAsync<AlbumRatingModel>(query, param: new
			    {
				    userId,
				    minimumRating,
				    trackRatingPercentage
			    }))
		    .ToList();
    }
}