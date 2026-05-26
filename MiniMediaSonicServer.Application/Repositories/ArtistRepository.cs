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
    
    public async Task<ArtistID3> GetArtistWithAlbumsByIdAsync(Guid artistId, Guid userId)
    {
        string query = @"SELECT 
    						a.ArtistId as Id,
    						a.Name as Name,
    						'artist_' || a.ArtistId as CoverArt,
    						'' as ArtistImageUrl,
    						album_count.albums as AlbumCount,
 							artist_rated.Rating as UserRating,
 							(case when artist_rated.Starred = true 
 							    then artist_rated.StarredAt 
 							    else null 
 							 end) as Starred,
    						
    						a.ArtistId as Id,
						 	al.AlbumId as Id,
						 	al.Title as Name,
						 	'' as version,
						 	a.Name as Artist,
						 	'album_' || al.AlbumId as CoverArt,
 							a.artistid AS ArtistId,
 							album_rated.Rating as UserRating,
 							(case when album_rated.Starred = true 
 							    then album_rated.StarredAt 
 							    else null 
 							 end) as Starred,
							al_sum.file_creationtime as Created,
							al_sum.Duration,
							al_sum.SongCount,
							al_sum.Year,
							al_sum.LastPlayDate as Played,
							al_sum.AlbumPlaycount as PlayCount
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
 						 left join sonicserver_artist_rated artist_rated on 
 						     artist_rated.ArtistId = a.ArtistId 
 						         and artist_rated.UserId = @userId
						     
						 left join sonicserver_album_rated album_rated on 
						     album_rated.AlbumId = al.AlbumId 
						     and album_rated.UserId = @userId
						     
						 JOIN lateral (
						     select count(ab.albumid) as albums 
						     from albums ab 
						     where ab.artistid = a.artistid 
						     limit 1) as album_count on true
						         
						 JOIN lateral (
						     select 
								    min(m.file_creationtime) as file_creationtime,
								    nullif(max(m.tag_year), 0) as Year, 
								    sum(EXTRACT(EPOCH FROM
									    (CASE WHEN length(m.Tag_Length) = 5 THEN '00:' || m.Tag_Length 
									    	ELSE m.Tag_Length END)::interval))::int as Duration,
						     		max(hist.UpdatedAt) as LastPlayDate,
									sum(hist.PlayCount) as AlbumPlaycount,
								    count(distinct(m.MetadataId)) as SongCount
							 from metadata m 
						     left join (
						         select TrackId, max(UpdatedAt) as UpdatedAt, count(*) as PlayCount
						         from sonicserver_user_playhistory
						         where UserId = @userId
						         group by TrackId
						     ) hist on hist.TrackId = m.MetadataId
						     where m.albumid = al.albumid
						     ) as al_sum on true
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
		        artistId,
		        userId
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
    
    public async Task<ArtistID3> GetArtistByIdAsync(Guid artistId, Guid userId)
    {
        string query = @"SELECT 
    						a.ArtistId as Id,
    						a.Name as Name,
    						'artist_' || a.ArtistId as CoverArt,
    						'' as ArtistImageUrl,
    						album_count.albums as AlbumCount,
 							artist_rated.Rating as UserRating,
 							(case when artist_rated.Starred = true 
 							    then artist_rated.StarredAt 
 							    else null 
 							 end) as Starred    						
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
 						 left join sonicserver_artist_rated artist_rated on 
 						     artist_rated.ArtistId = a.ArtistId 
 						         and artist_rated.UserId = @userId
						 JOIN lateral (
						     select count(ab.albumid) as albums 
						     from albums ab 
						     where ab.artistid = a.artistid 
						     limit 1) as album_count on true
						                             
						 where a.ArtistId = @artistId
						 limit 1";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        return await conn.QueryFirstOrDefaultAsync<ArtistID3>(query, 
	        param: new
	        {
		        artistId,
		        userId
	        });
    }
    
    public async Task<List<ArtistID3>> GetAllArtistsAsync(Guid userId)
    {
	    string query = @"SELECT 
							a.ArtistId as Id,
							a.Name as Name,
							'artist_' || a.ArtistId as CoverArt,
							'' as ArtistImageUrl,
							al_sum.albums as AlbumCount,
							artist_rated.Rating as UserRating,
							(case when artist_rated.Starred = true 
							    then artist_rated.StarredAt 
							    else null 
							 end) as Starred
							--musicbrainzid
						 FROM artists a
						 left join sonicserver_artist_rated artist_rated on 
 							artist_rated.ArtistId = a.ArtistId 
 							and artist_rated.UserId = @userId
 							
						 JOIN lateral (
	 							select 
	 								count(ab.albumid) as albums
	 							from albums ab 
	 							where ab.artistid = a.artistid
 							) as al_sum on true
						 order by a.Name asc";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return (await conn.QueryAsync<ArtistID3>(query, param: new
		    {
			    userId
		    }))
		    .ToList();
    }
    
    public async Task<List<ArtistID3>> GetStarredArtistsAsync(Guid userId)
    {
	    string query = @"SELECT
							a.ArtistId as Id,
							a.Name as Name,
							'artist_' || a.ArtistId as CoverArt,
							'' as ArtistImageUrl,
							album_count.albums as AlbumCount,
							artist_rated.Rating as UserRating,
							(case when artist_rated.Starred = true 
							    then artist_rated.StarredAt 
							    else null 
							 end) as Starred
							--musicbrainzid
						 FROM artists a
						 join sonicserver_artist_rated artist_rated on 
 							artist_rated.ArtistId = a.ArtistId 
 							and artist_rated.UserId = @userId
 							and artist_rated.Starred = true
 							
						 JOIN lateral (
 							select 
 								count(ab.albumid) as albums 
 							from albums ab 
 							where ab.artistid = a.artistid
 							) as album_count on true
						 order by a.Name asc";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return (await conn.QueryAsync<ArtistID3>(query, param: new
		    {
			    userId
		    }))
		    .ToList();
    }
    public async Task<LastFmArtist> GetLastFmArtistInfoAsync(string artistName, int maxSimilarArtists)
    {
        string query = @"SELECT 
    						la.ArtistId,
    						la.LastFmId,
    						la.Name,
    						la.OnTour,
    						la.StatsListeners,
    						la.MusicBrainzId,
    						la.BioContent,
    						la.BioSummary,
    						la.BioYearFormed,
    						la.BioPublished,
    						la.Uri,
    						la.LastSyncTime,
    						la_image.Uri as ImageUri,
    						a.ArtistId
						 FROM lastfm_artist la
						 LEFT JOIN lastfm_artist_image la_image ON la_image.ArtistId = la.ArtistId
						 LEFT JOIN lastfm_artist_similar las ON las.ArtistId = la.ArtistId
						 LEFT JOIN lastfm_artist sim_la ON sim_la.ArtistId = las.SimilarArtistId
						 LEFT JOIN artists a ON lower(a.Name) = lower(sim_la.Name) --need to improve this later with table cross-referencing
						 where lower(la.Name) = lower(@artistName)
						 limit @maxSimilarArtists";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = (await conn.QueryAsync<LastFmArtist, Guid?, LastFmArtist>(query, 
	        map: (artist, similarArtistId) =>
	        {
		        if (similarArtistId.HasValue && similarArtistId != Guid.Empty)
		        {
			        artist.SimilarArtistIds.Add(similarArtistId.Value);
		        }
		        return artist;
	        },
	        splitOn: "ArtistId, ArtistId",
	        param: new
	        {
		        artistName,
		        maxSimilarArtists
	        })).ToList();

        return results
	        .GroupBy(artist => artist.Name)
	        .Select(group =>
	        {
		        var artist = group.First();
		        artist.SimilarArtistIds = group.SelectMany(artist => artist.SimilarArtistIds).ToList();
		        return artist;
	        })
	        .FirstOrDefault();
    }
    
    public async Task<string?> GetArtistNameByArtistIdAsync(Guid artistId)
    {
	    string query = @"SELECT a.Name
						 FROM artists a
						 where a.ArtistId = @artistId
						 limit 1";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return await conn.QueryFirstOrDefaultAsync<string>(query, 
		    param: new
		    {
			    artistId
		    });
    }
    public async Task<string?> GetArtistNameByAlbumIdAsync(Guid albumId)
    {
	    string query = @"SELECT a.Name
						 FROM albums ab
						 join artists a on a.ArtistId = ab.ArtistId
						 where ab.AlbumId = @albumId
						 limit 1";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return await conn.QueryFirstOrDefaultAsync<string>(query, 
		    param: new
		    {
			    albumId
		    });
    }
    public async Task<string?> GetArtistNameByTrackIdAsync(Guid trackId)
    {
	    string query = @"SELECT a.Name
					     FROM metadata m
						 join albums ab on ab.AlbumId = m.AlbumId
						 join artists a on a.ArtistId = ab.ArtistId
						 where m.MetadataId = @trackId
						 limit 1";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return await conn.QueryFirstOrDefaultAsync<string>(query, 
		    param: new
		    {
			    trackId
		    });
    }
}