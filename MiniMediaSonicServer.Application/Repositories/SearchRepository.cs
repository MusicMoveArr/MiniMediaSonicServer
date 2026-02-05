using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class SearchRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    
    public SearchRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<List<ArtistID3>> SearchArtistsAsync(string searchquery, int count, int offset)
    {
	    string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
						 SELECT distinct on (a.record_id)
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
						 FROM artists a
						 left join sonicserver_indexed_search search on search.Id = a.ArtistId
 						 left join sonicserver_artist_rated artist_rated on artist_rated.ArtistId = a.ArtistId
						 JOIN albums al ON al.artistid = a.artistid
						 JOIN lateral (select * from metadata m where m.albumid = al.albumid order by m.tag_year desc limit 1) as m on true
						 JOIN lateral (select count(ab.albumid) as albums from albums ab where ab.artistid = a.artistid limit 1) as album_count on true
						 where search.SearchTerm % lower(@searchquery)
					     offset @offset
						 limit @count";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();
        var transaction = await conn.BeginTransactionAsync();
        var results = new  List<ArtistID3>();

        try
        {
	        results = (await conn.QueryAsync<ArtistID3>(query, 
		        param: new
		        {
			        searchquery,
			        count,
			        offset
		        },
		        transaction: transaction)).ToList();
        }
        catch (Exception ex)
        {
	        Console.WriteLine($"Parameters, searchquery='{searchquery}', count='{count}', Error={ex.Message}\r\nStackTrace={ex.StackTrace}");
        }
        finally
        {
	        await transaction.CommitAsync();
        }

        return results;
    }
    
    public async Task<List<AlbumID3>> SearchAlbumsAsync(string searchquery, int count, int offset, Guid userId)
    {
	    string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
						 SELECT distinct on (al.AlbumId)
						 	al.AlbumId as Id,
						 	al.Title as Name,
						 	'' as version,
						 	a.Name as Artist,
						 	m.tag_year as year,
						 	'album_' || al.AlbumId as CoverArt,
 							a.artistid AS ArtistId,
							m.file_creationtime as Created,
 							album_rated.Rating as UserRating,
 							(case when album_rated.Starred = true 
 							    then album_rated.UpdatedAt 
 							    else null 
 							 end) as Starred,
							search.SearchTerm
						 FROM albums al
						 join sonicserver_indexed_search search on search.Id = al.AlbumId
						 JOIN artists a on a.ArtistId = al.ArtistId
 						 left join sonicserver_album_rated album_rated on album_rated.AlbumId = al.AlbumId and album_rated.UserId = @userId
						 LEFT JOIN lateral (select * from metadata m where m.albumid = al.albumid order by m.tag_year desc limit 1) as m on true
						 LEFT JOIN lateral (
						     select m.file_creationtime as file_creationtime 
						     from metadata m 
						     where m.albumid = al.albumid 
						     order by m.file_creationtime desc
						     limit 1) as recent_m on true
						 where search.SearchTerm % lower(@searchquery)
						 offset @offset
						 limit @count";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.OpenAsync();
	    var transaction = await conn.BeginTransactionAsync();
	    var results = new  List<AlbumID3>();

	    try
	    {
		    results = (await conn.QueryAsync<AlbumID3>(query, 
			    param: new
			    {
				    searchquery,
				    count,
				    offset,
				    userId
			    },
			    transaction: transaction)).ToList();
	    }
	    catch (Exception ex)
	    {
		    Console.WriteLine($"Parameters, searchquery='{searchquery}', count='{count}', Error={ex.Message}\r\nStackTrace={ex.StackTrace}");
	    }
	    finally
	    {
		    await transaction.CommitAsync();
	    }
	    
	    return results;
    }
    
    public async Task<List<TrackID3>> SearchTracksAsync(string searchquery, int count, int offset)
    {
	    string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
						 SELECT 
 							m.MetadataId as TrackId,
 							al.AlbumId as Parent,
 							al.AlbumId as AlbumId,
 							a.artistid AS ArtistId,
						 	'album_' || m.MetadataId as CoverArt,
 							m.Title as Title,
 							al.Title as Album,
 							a.Name as Artist,
 							m.Tag_Track as TrackNumber,
 							m.Tag_Year as Year,
 							'' as Genre,
 							0 as Size,
 							case 
 								when m.Path ilike '%.mp3' then 'audio/mpeg'
 								when m.Path ilike '%.m4a' then 'audio/mp4'
 								when m.Path ilike '%.mp4' then 'audio/mp4'
 								when m.Path ilike '%.flac' then 'audio/flac'
 								when m.Path ilike '%.ogg' then 'audio/ogg'
 								when m.Path ilike '%.opus' then 'audio/ogg'
 								when m.Path ilike '%.wav' then 'audio/wav'
 								else 'application/octet-stream'
 							end as ContentType,
 							regexp_substr(m.Path, '([a-zA-Z0-9]{2,5})$') as Suffix,
							m.tag_isrc as Isrc_Single,
							m.Path as Path,
							'music' AS Type,
							'song' AS MediaType,
 							    
 							EXTRACT(EPOCH FROM
							    (CASE WHEN length(m.Tag_Length) = 5 THEN '00:' || m.Tag_Length 
							    ELSE m.Tag_Length END)::interval) AS Duration,
 							    
							m.file_creationtime as Created,
							t.tags->>'bitrate' AS BitRate,
							16 as BitDepth,
							44100 as SamplingRate,
							2 as ChannelCount,

 							track_rated.Rating as UserRating,
 							(case when track_rated.Starred = true 
 							    then track_rated.UpdatedAt 
 							    else null 
 							 end) as Starred,

							regexp_substr(t.tags->>'bpm', '[0-9]+([0-9]+)?') as BPM,
							regexp_substr(t.tags->>'replaygain_track_gain', '-?[0-9]+(\.[0-9]+)?') as TrackGain,
							regexp_substr(t.tags->>'replaygain_album_gain', '-?[0-9]+(\.[0-9]+)?') as AlbumGain,
							regexp_substr(t.tags->>'replaygain_track_peak', '-?[0-9]+(\.[0-9]+)?') as TrackPeak,
							regexp_substr(t.tags->>'replaygain_album_peak', '-?[0-9]+(\.[0-9]+)?') as AlbumPeak,
 							    
							joined_artist.ArtistId as Id,
							joined_artist.Name
						 FROM metadata m
						 JOIN albums al ON al.AlbumId = m.AlbumId
						 JOIN artists a on a.ArtistId = al.ArtistId
						 left join sonicserver_indexed_search search on search.Id = m.MetadataId
						 left join sonicserver_track_rated track_rated on track_rated.TrackId = m.MetadataId
 							    
 						 left join lateral (
 							select DISTINCT unnest(string_to_array(
							        replace(replace(
									            COALESCE(tag_alljsontags->>'Artists', tag_alljsontags->>'ARTISTS'), 
									            '&', ';'), 
									            '/', ';'),
									        ';'
									    )) AS artist
						 ) all_artists ON true
 						left join lateral (
 							select artistid, name 
 							from artists join_artist 
 							where lower(join_artist.name) = lower(all_artists.artist)
 							limit 1) joined_artist on true
 							    
 						 LEFT JOIN LATERAL (
						    SELECT jsonb_object_agg(lower(key), value) AS tags
						    FROM jsonb_each_text(m.tag_alljsontags)
						  ) t ON TRUE

						 where search.SearchTerm % lower(@searchquery)
						 offset @offset
						 limit @count";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.OpenAsync();
	    var transaction = await conn.BeginTransactionAsync();
	    var results = new  List<TrackID3>();

	    try
	    {
		    results = (await conn.QueryAsync<TrackID3, ReplayGain, ArtistID3, TrackID3>(query,
			    (track, replayGain, extraArtist) =>
			    {
				    track.ReplayGain = replayGain;

				    if (!string.IsNullOrWhiteSpace(track.Isrc_Single))
				    {
					    track.Isrc.Add(track.Isrc_Single);
				    }

				    if (!track.Artists.Any(a => a.Id == track.ArtistId))
				    {
					    track.Artists.Add(new NameIdEntity(track.ArtistId, track.Artist));
				    }

				    if (extraArtist != null && !track.Artists.Any(a => a.Id == extraArtist.Id))
				    {
					    track.Artists.Add(new NameIdEntity(extraArtist.Id, extraArtist.Name));
				    }
				    return track;
			    },
			    splitOn: "TrackId, TrackGain, Id",
			    param: new
			    {
				    searchquery,
				    count,
				    offset
			    })).ToList();
	    }
	    catch (Exception ex)
	    {
		    Console.WriteLine($"Parameters, searchquery='{searchquery}', count='{count}', Error={ex.Message}\r\nStackTrace={ex.StackTrace}");
	    }
	    finally
	    {
		    await transaction.CommitAsync();
	    }

	    var groupedResult = results
		    .GroupBy(track => track.TrackId)
		    .Select(group =>
		    {
			    var track = group.First();
			    track.Isrc = group.SelectMany(t => t.Isrc)
				    .Distinct()
				    .ToList();
			    
			    track.Artists = group.SelectMany(t => t.Artists)
				    .DistinctBy(a => a.Id)
				    .ToList();
			    return track;
		    })
		    .ToList();

	    return groupedResult;
    }
    
    
    public async Task<ID3Type?> GetID3TypeAsync(Guid id)
    {
	    string query = @"select
						 	case 
						 	    when exists (select 1 from artists where artistid = @id)  then 1
						 		when exists (select 1 from albums where albumid = @id) then 2
						 		when exists (select 1 from metadata where metadataid = @id) then 3
						 		else 0
						 	end";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return await conn.QueryFirstOrDefaultAsync<ID3Type>(query,
		    param: new
		    {
			    id
		    });
    }
}