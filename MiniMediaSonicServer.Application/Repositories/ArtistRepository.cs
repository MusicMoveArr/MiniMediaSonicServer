using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class ArtistRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public ArtistRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<ArtistID3> GetArtistByIdAsync(Guid artistId)
    {
        string query = @"SELECT 
    						a.ArtistId as Id,
    						a.Name as Name,
    						'artist_' || a.ArtistId as CoverArt,
    						'' as ArtistImageUrl,
    						album_count.albums as AlbumCount,
 							artist_rated.Rating as UserRating,
 							(case when artist_rated.Starred = true 
 							    then artist_rated.UpdatedAt 
 							    else null 
 							 end) as Starred,
    						
    						a.ArtistId as Id,
						 	al.AlbumId as Id,
						 	al.Title as Name,
						 	'' as version,
						 	a.Name as Artist,
						 	m.tag_year as year,
						 	'album_' || al.AlbumId as CoverArt,
 							a.artistid AS ArtistId,
							m.file_creationtime as Created
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
 						 left join sonicserver_artist_rated artist_rated on artist_rated.ArtistId = a.ArtistId
						 JOIN lateral (select * from metadata m where m.albumid = al.albumid order by m.tag_year desc limit 1) as m on true
						 JOIN lateral (select count(ab.albumid) as albums from albums ab where ab.artistid = a.artistid limit 1) as album_count on true
						 where a.ArtistId = @artistId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = (await conn.QueryAsync<ArtistID3, AlbumID3, ArtistID3>(query, 
	        map: (artist, album) =>
	        {
		        if (artist.Albums == null)
		        {
			        artist.Albums = new List<AlbumID3>();
		        }
		        artist.Albums.Add(album);
		        return artist;
	        },
	        splitOn: "Id, Id",
	        param: new
	        {
		        artistId
	        })).ToList();

        
        var groupedResult = results
	        .GroupBy(artist => artist.Id)
	        .Select(group =>
	        {
		        var artist = group.First();
		        artist.Albums = group.SelectMany(artist => artist.Albums).ToList();
		        foreach (var album in artist.Albums)
		        {
			        album.Artists = [new NameIdEntity(album.ArtistId, album.Artist)];
		        }
		        return artist;
	        })
	        .ToList();

        return groupedResult.FirstOrDefault();
    }
    
    public async Task<List<ArtistID3>> GetAllArtistsAsync()
    {
	    string query = @"SELECT distinct on (a.ArtistId)
    						a.ArtistId as Id,
    						a.Name as Name,
    						'artist_' || a.ArtistId as CoverArt,
    						'' as ArtistImageUrl,
    						album_count.albums as AlbumCount,
 							artist_rated.Rating as UserRating,
 							(case when artist_rated.Starred = true 
 							    then artist_rated.UpdatedAt 
 							    else null 
 							 end) as Starred
    						--musicbrainzid
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
 						 left join sonicserver_artist_rated artist_rated on artist_rated.ArtistId = a.ArtistId
						 JOIN lateral (select count(ab.albumid) as albums from albums ab where ab.artistid = a.artistid limit 1) as album_count on true";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return (await conn.QueryAsync<ArtistID3>(query))
		    .ToList();
    }
}