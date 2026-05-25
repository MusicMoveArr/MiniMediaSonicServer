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
    
    public async Task<List<ArtistID3>> SearchArtistsAsync(string searchquery, int count, int offset, Guid userId)
    {
	    string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
						 SELECT
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
							 a.record_id
						 FROM sonicserver_indexed_search search 
						 join artists a on a.ArtistId = search.Id
						 left join sonicserver_artist_rated artist_rated on 
							 artist_rated.ArtistId = a.ArtistId 
							 and artist_rated.UserId = @userId
						 JOIN lateral (
							 select count(ab.albumid) as albums 
							 from albums ab 
							 where ab.artistid = a.artistid
						 ) as album_count on true
 
						 where search.SearchTerm % lower(@searchquery) and search.type = 'artist'
						 order by similarity(search.SearchTerm, lower(@searchquery)) desc
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
    
    public async Task<List<AlbumID3>> SearchAlbumsAsync(string searchquery, int count, int offset, Guid userId)
    {
	    string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
						 with album_collection as (
							SELECT search.Id, search.SearchTerm
							FROM sonicserver_indexed_search search
							where search.SearchTerm % lower(@searchquery) and search.type = 'album'
							offset @offset
							limit @count
						 ),
						 al_sum_cte AS (
						     SELECT
						         m.albumid,
						         min(m.file_creationtime) AS file_creationtime,
						         nullif(max(m.tag_year), 0) AS Year,
						         sum(EXTRACT(EPOCH FROM
						             (CASE WHEN length(m.Tag_Length) = 5 
						                   THEN '00:' || m.Tag_Length
						                   ELSE m.Tag_Length END)::interval))::int AS Duration,
						         max(hist.UpdatedAt) AS LastPlayDate,
						         sum(hist.PlayCount) AS AlbumPlaycount,
						         count(distinct m.MetadataId) AS SongCount
						     FROM album_collection col
						     JOIN metadata m ON m.albumid = col.Id
						     LEFT JOIN (
						         SELECT TrackId,
						                max(UpdatedAt)  AS UpdatedAt,
						                count(*)        AS PlayCount
						         FROM sonicserver_user_playhistory
						         WHERE UserId = @userId
						           AND Scrobble = true
						         GROUP BY TrackId
						     ) hist ON hist.TrackId = m.MetadataId
						     GROUP BY m.albumid
						 ),
						 al_dups as (
							SELECT lower(ab.title) as title, true as HasDuplicates
							FROM album_collection col
						    join albums ab on ab.albumid = col.id
						    JOIN albums ab2 ON ab2.albumid != col.id
						    WHERE lower(ab.title) = lower(ab2.title)
						    GROUP BY lower(ab.title)
						    HAVING count(*) > 1
						 )
						 SELECT
							al.AlbumId as Id,
							al.Title as Name,
							'' as version,
							a.Name as Artist,
							al_sum.Year as year,
							'album_' || al.AlbumId as CoverArt,
							a.artistid AS ArtistId,
							al_sum.file_creationtime as Created,
							al_sum.Duration,
							al_sum.SongCount as songCount,
							al_sum.AlbumPlaycount as PlayCount,
							album_rated.Rating as UserRating,
							(case when album_rated.Starred = true 
							    then album_rated.StarredAt 
							    else null 
							 end) as Starred,
							dups.HasDuplicates as HasDuplicates
						 FROM album_collection col
						 join albums al on al.AlbumId = col.Id
						 JOIN artists a on a.ArtistId = al.ArtistId
						 left join sonicserver_album_rated album_rated on 
							album_rated.AlbumId = al.AlbumId 
							and album_rated.UserId = @userId
							
						 JOIN al_sum_cte al_sum ON al_sum.albumid = al.albumid
						 left join al_dups dups on dups.title = lower(al.title)
						 order by similarity(col.SearchTerm, lower(@searchquery)) desc";

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

	    var finalResults = results
		    .Where(r => !r.HasDuplicates)
		    .ToList();
	    
	    finalResults.AddRange(results
		    .Where(r => r.HasDuplicates)
		    .GroupBy(r => r.Name.ToLower())
		    .Select(r => r.First()));
	    
	    return finalResults;
    }
    
    public async Task<List<TrackID3>> SearchTracksAsync(string searchquery, int count, int offset, Guid userId, int accuracy = 50)
    {
	    string query = @$"SET LOCAL pg_trgm.similarity_threshold = 0.{accuracy};
						with tracks as (
							SELECT search.Id, search.SearchTerm
							FROM sonicserver_indexed_search search
							where search.SearchTerm % lower(@searchquery) and search.type = 'track'
							order by similarity(search.SearchTerm, lower(@searchquery)) desc
							offset @offset
							limit @count
						),
						 playhist as (
							 select	max(hist.UpdatedAt) as LastPlayDate,
									sum(case when hist.Scrobble = true then 1 else 0 end) as PlayCount
							 from sonicserver_user_playhistory hist
							 join tracks track on track.Id = hist.TrackId
							 where hist.UserId = @userId
						)
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
							 NULLIF(m.tag_year, 0) as Year,
							 m.Computed_Genre as Genre,
							 m.File_Size as Size,
							 case when m.Path ilike '%.mp3' then 'audio/mpeg'
								  when m.Path ilike '%.m4a' then 'audio/mp4'
								  when m.Path ilike '%.mp4' then 'audio/mp4'
								  when m.Path ilike '%.flac' then 'audio/flac'
								  when m.Path ilike '%.ogg' then 'audio/ogg'
								  when m.Path ilike '%.opus' then 'audio/ogg'
								  when m.Path ilike '%.wav' then 'audio/wav'
								  else 'application/octet-stream'
							 end as ContentType,
							 regexp_substr(m.Path, '([a-zA-Z0-9]{{2,5}})$') as Suffix,
							 m.tag_isrc as Isrc_Single,
							 m.Path as Path,
							 'music' AS Type,
							 'song' AS MediaType,
							 playhist.LastPlayDate as Played,
							 playhist.PlayCount as PlayCount,

							 EXTRACT(EPOCH FROM
							 (CASE WHEN length(m.Tag_Length) = 5 THEN '00:' || m.Tag_Length 
								ELSE m.Tag_Length END)::interval) AS Duration,

							 m.file_creationtime as Created,
							 m.computed_bitrate AS BitRate,
							 16 as BitDepth,
							 44100 as SamplingRate,
							 2 as ChannelCount,

							 track_rated.Rating as UserRating,
							 (case when track_rated.Starred = true 
								 then track_rated.StarredAt 
								 else null 
							 end) as Starred,

							m.computed_bitrate as BPM,
							m.computed_replaygain_track_gain as TrackGain,
							m.computed_replaygain_album_gain as AlbumGain,
							m.computed_replaygain_track_peak as TrackPeak,
							m.computed_replaygain_album_peak as AlbumPeak,

							 joined_artist.ArtistId as Id,
							 joined_artist.Name
							 
						 FROM tracks track
						 join metadata m on m.MetadataId = track.Id
						 JOIN albums al ON al.AlbumId = m.AlbumId
						 JOIN artists a on a.ArtistId = al.ArtistId
						 left join sonicserver_track_rated track_rated on 
							 track_rated.TrackId = m.MetadataId 
							 and track_rated.UserId = @userId

						 left join lateral (
							select DISTINCT unnest(string_to_array(coalesce(m.tag_alljsontags->>'artists', m.tag_alljsontags->>'ARTISTS'), ';')) AS artist
						 ) all_artists ON true
						 left join lateral (
							 select artistid, name 
							 from artists join_artist 
							 where lower(join_artist.name) = lower(all_artists.artist)
							 limit 1) joined_artist on true

						left join playhist on true ";

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
				    offset,
				    accuracy,
				    userId
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
    
    
    public async Task<ID3Type> GetId3TypeAsync(Guid id)
    {
	    string query = @"select
						 	case 
						 	    when exists (select 1 from artists where artistid = @id)  then 1
						 		when exists (select 1 from albums where albumid = @id) then 2
						 		when exists (select 1 from metadata where metadataid = @id) then 3
						 		when exists (select 1 from sonicserver_playlist where playlistid = @id) then 4
						 		else 0
						 	end";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return (await conn.QueryFirstOrDefaultAsync<ID3Type?>(query,
		    param: new
		    {
			    id
		    })) ?? ID3Type.Unknowm;
    }
}