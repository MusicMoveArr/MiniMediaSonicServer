using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Models.Database;
using Npgsql;

namespace MiniMediaSonicServer.WebJob.AutoLike.Application.Repositories;

public class AutoRateDuplicateTracksRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public AutoRateDuplicateTracksRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<List<RateDuplicateTrackModel>> GetRatedTracksAsync(Guid userId)
    {
	    string query = @$"SELECT 
							  m.metadataid as MetadataId,
							  a.name as ArtistName,
							  m.Title as TrackTitle,
							  rt.rating as Rating,
							  rt.ratedat as RatedAt,
							  m.tag_acoustidfingerprint as AcoustIdFingerprint
						  FROM sonicserver_track_rated rt
						  join metadata m on m.metadataid = rt.trackid
						  join albums al on al.albumid = m.albumid 
						  join artists a on a.artistid = al.artistid 
						  where
  							rt.userid = @userId
  							and rt.rating > 0
  							and length(m.tag_acoustidfingerprint) > 0";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.OpenAsync();

	    return (await conn
			    .QueryAsync<RateDuplicateTrackModel>(query, param: new
			    {
				    userId
			    }))
		    .ToList();
    }
    
    public async Task<List<TrackModel>> TracksByArtistNameAsync(Guid userId, string artistName)
    {
	    string query = @$"select a.name as ArtistName, 
							a.artistid as ArtistId, 
							al.title as AlbumTitle, 
							al.albumid as AlbumId, 
							m.metadataid as MetadataId, 
							m.Title as TrackTitle, 
							m.tag_acoustidfingerprint as AcoustIdFingerprint,
							coalesce(rt.rating, 0) as Rating,
							rt.ratedat as RatedAt
						  from artists a
						  join albums al on al.artistid = a.artistid 
						  join metadata m on m.albumid = al.albumid
						  left join sonicserver_track_rated rt on rt.trackid = m.metadataid and rt.UserId = @userId
						  where lower(a.name) = lower(@artistName)
						  and length(m.tag_acoustidfingerprint) > 0";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.OpenAsync();

	    return (await conn
			    .QueryAsync<TrackModel>(query, param: new
			    {
				    userId,
				    artistName
			    }))
		    .ToList();
    }
}