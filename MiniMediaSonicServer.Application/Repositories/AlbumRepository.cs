using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class AlbumRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public AlbumRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<List<AlbumID3>> GetAlbumId3Async(GetAlbumList2Request request, Guid userId)
    {
        string query = @"WITH album_playhistory AS (
						     SELECT
						         m.AlbumId,
						         COUNT(*) AS AlbumPlaycount,
						         MAX(playhistory.UpdatedAt) AS UpdatedAt
						     FROM sonicserver_user_playhistory playhistory
						     JOIN metadata m ON m.MetadataId = playhistory.TrackId
						     WHERE playhistory.UserId = @userId and playhistory.Scrobble = true
						     GROUP BY m.AlbumId
						 ),
						 candidate_albums AS (
						     SELECT
						         al.AlbumId,
						         al.Title,
						         al.artistid,
						         al.record_id,
						         al.year,
						         album_rated.Rating AS UserRating,
						         album_rated.Starred,
						         album_rated.StarredAt,
						         playhistory.AlbumPlaycount,
						         playhistory.UpdatedAt AS PlayUpdatedAt
						     FROM albums al TABLESAMPLE SYSTEM(CASE WHEN @type = 'random' THEN 1 ELSE 100 END)
						     LEFT JOIN sonicserver_album_rated album_rated
						            ON album_rated.AlbumId = al.AlbumId
						           AND album_rated.UserId = @userId
						     LEFT JOIN album_playhistory playhistory ON playhistory.AlbumId = al.AlbumId
						     
						     WHERE (CASE
						                WHEN @type = 'byGenre'  THEN exists(
						                    select 1 from metadata m
						                    where m.albumid = al.albumid
						                      and m.computed_genre ILIKE '%' || @genre || '%')
						                WHEN @type IN ('recent', 'frequent') THEN playhistory.AlbumId IS NOT NULL
						                WHEN @type IN ('newest', 'random') THEN 1=1
						                WHEN @type = 'starred' THEN album_rated.Starred
						                WHEN @type = 'byYear' THEN al.year between @toYear and @fromYear
						                WHEN @type = 'alphabeticalByName' THEN al.record_title_asc_id >= @offset AND al.record_title_asc_id <= @offset + @limit
						                ELSE al.record_id >= @offset AND al.record_id <= @offset + @limit
						            END)
						     ORDER BY
						         CASE WHEN @type = 'frequent' THEN playhistory.AlbumPlaycount END DESC,
						         CASE WHEN @type = 'random' THEN random() END DESC,
						         CASE WHEN @type = 'byYear' THEN al.year END DESC,
						         CASE WHEN @type = 'newest' THEN al.record_id END DESC,
						         CASE WHEN @type = 'alphabeticalByName' THEN al.record_title_asc_id END ASC,
						         CASE WHEN @type = 'recent' THEN playhistory.UpdatedAt END DESC,
						         CASE WHEN @type = 'starred' THEN album_rated.StarredAt END DESC

							 OFFSET (case when not @type IN ('alphabeticalByName') then @offset else 0 end)
						     LIMIT @limit
						 )
						 SELECT
						     ca.AlbumId AS Id,
						     ca.Title AS Name,
						     '' AS version,
						     a.Name AS Artist,
						     'album_' || ca.AlbumId AS CoverArt,
						     a.artistid AS ArtistId,
						     al_sum.file_creationtime_min AS Created,
						     al_sum.Duration,
						     al_sum.SongCount,
						     NULLIF(ca.year, 0) as Year,
						     COALESCE(ca.AlbumPlaycount, 0) AS PlayCount,
						     ca.UserRating,
						     CASE WHEN ca.Starred = true
						 		THEN ca.StarredAt 
						 		ELSE NULL 
						 	END AS Starred
						 FROM candidate_albums ca
						 JOIN artists a ON a.artistid = ca.artistid
 
						 JOIN LATERAL (
						     SELECT
						         min(m.file_creationtime) AS file_creationtime_min,
						         sum(EXTRACT(EPOCH FROM
						             (CASE WHEN length(m.Tag_Length) = 5
						                   THEN '00:' || m.Tag_Length
						                   ELSE m.Tag_Length END)::interval))::int AS Duration,
						         count(distinct m.MetadataId) AS SongCount
						     FROM metadata m
						     WHERE m.albumid = ca.AlbumId
						 ) AS al_sum ON true
						 ORDER by CASE WHEN @type = 'byYear' THEN ca.year END DESC";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = (await conn.QueryAsync<AlbumID3>(query, 
	        param: new
	        {
		        limit = request.Size,
		        type = request.Type,
		        offset = request.Offset,
		        genre = request.Genre,
		        fromYear = request.FromYear,
		        toYear = request.ToYear,
		        userId
	        })).ToList();

        foreach (var result in results)
        {
	        result.Artists = [new NameIdEntity(result.ArtistId, result.Artist)];
        }

        return results;
    }
    
    public async Task<AlbumID3?> GetAlbumId3WithTracksAsync(Guid albumId, Guid userId)
    {
	    string query = @"WITH album_collection AS (
						    SELECT unnest(array_agg(ab.albumid)) AS albumid
						    FROM albums ab
						    JOIN albums ab2 ON ab2.albumid = @id
						    JOIN metadata m ON m.albumid = ab2.albumid
						    WHERE lower(ab.title) = lower(ab2.title)
						        AND ab.title ~ '[0-9]'
						        AND lower(ab.title) NOT IN ('best of', 'the best of', 'live', 'greatest hits', '[unknown]')
						    GROUP BY lower(ab.title)
						    HAVING count(*) > 1
						       AND count(m.metadataid) < 200

						    UNION

						    SELECT @id AS albumid
						),
						deduped_metadata AS (
						    SELECT DISTINCT ON (m.AlbumId, m.Title)
						        m.*
						    FROM metadata m
						    LEFT JOIN sonicserver_track_rated track_rated ON track_rated.TrackId = m.MetadataId and track_rated.UserId = @userId
						    WHERE m.albumid IN (SELECT albumid FROM album_collection)
						    ORDER BY m.AlbumId, m.Title, track_rated.ratedat DESC NULLS LAST
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
						    FROM deduped_metadata m
						    LEFT JOIN (
						        SELECT TrackId,
						               max(UpdatedAt) AS UpdatedAt,
						               count(*)       AS PlayCount
						        FROM sonicserver_user_playhistory
						        WHERE UserId  = @userId
						          AND Scrobble = true
						        GROUP BY TrackId
						    ) hist ON hist.TrackId = m.MetadataId
						    GROUP BY m.albumid
						)
						 SELECT 
 							al.AlbumId as Id,
 							al.Title as Name,
 							'' as version,
 							a.Name as Artist,
 							NULLIF(m.tag_year, 0) as Year,
						 	'album_' || al.AlbumId as CoverArt,
 							a.artistid AS ArtistId,
 							al_sum.file_creationtime as Created,
							al_sum.Duration,
							al_sum.SongCount,
							al_sum.Year,
 							album_rated.Rating as UserRating,
 							playhistory.LastPlayDate as Played,
							al_sum.AlbumPlaycount as PlayCount,
 							
 							(case when album_rated.Starred = true 
 							    then album_rated.StarredAt 
 							    else null 
 							 end) as Starred,
 							
 							m.MetadataId as TrackId,
 							al.AlbumId as Parent,
 							al.AlbumId as AlbumId,
 							a.artistid AS ArtistId,
						 	'album_' || m.MetadataId as CoverArt,
 							m.Title as Title,
 							al.Title as Album,
 							a.Name as Artist,
 							m.Tag_Track as TrackNumber,
 							m.Tag_Disc as DiscNumber,
 							NULLIF(m.tag_year, 0) as Year,
 							m.Computed_Genre as Genre,
 							m.File_Size as Size,
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
 							playhistory.LastPlayDate as Played,
 							playhistory.PlayCount as PlayCount,
 							    
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
						 FROM album_collection col
						 JOIN albums al ON al.albumid = col.albumid
					     join artists a on a.artistid = al.artistid
						 JOIN deduped_metadata m on m.albumid = al.albumid
 						 left join sonicserver_track_rated track_rated on track_rated.TrackId = m.MetadataId and track_rated.UserId = @userId
 						 left join sonicserver_album_rated album_rated on album_rated.AlbumId = al.AlbumId and album_rated.UserId = @userId
 						 
						 left join lateral (
							select DISTINCT unnest(string_to_array(coalesce(m.tag_alljsontags->>'artists', m.tag_alljsontags->>'ARTISTS'), ';')) AS artist
						 ) all_artists ON true
 							    
 						left join lateral (
 							select artistid, name 
 							from artists join_artist 
 							where lower(join_artist.name) = lower(all_artists.artist)
 							limit 1) joined_artist on true
 							    
 						 left join lateral (
 							select 	max(hist.UpdatedAt) as LastPlayDate,
    								sum(case when hist.Scrobble = true then 1 else 0 end) as PlayCount
 							from sonicserver_user_playhistory hist
 							where hist.UserId = @userId and hist.TrackId = m.MetadataId
 							    ) playhistory on true
 							    
						 JOIN al_sum_cte al_sum ON al_sum.albumid = al.albumid";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    var results = await conn.QueryAsync<AlbumID3, TrackID3, ReplayGain, ArtistID3, AlbumID3>(query,
		    (album, track, replayGain, extraArtist) =>
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
			    
			    album.Artists = [new NameIdEntity(album.ArtistId, album.Artist)];
			    album.Song.Add(track);
			    return album;
		    },
		    splitOn: "TrackId, TrackGain, Id",
		    param: new
		    {
			    id = albumId,
			    userId
		    });

	    var groupedResult = results
		    .GroupBy(album => new { })
		    .Select(group =>
		    {
			    var album = group.First(a => a.Id == albumId);
			    album.Duration = group.Sum(a => album.Duration);
			    album.PlayCount = group.Sum(a => album.PlayCount);
			    album.SongCount = group.Sum(a => album.SongCount);
			    album.Artists = group
				    .SelectMany(a => album.Artists)
				    .DistinctBy(a => a.Id)
				    .ToList();

			    if (group.Select(a => a.Id).Distinct().Count() > 1)
			    {
				    album.IsCompilation = true;
			    }
			    
			    album.Song = group
				    .SelectMany(album => album.Song)
				    .DistinctBy(track => track.TrackId)
				    .OrderBy(track => track.DiscNumber)
				    .ThenBy(track => track.TrackNumber)
				    .ToList();

			    foreach (var song in album.Song)
			    {
				    song.AlbumId = album.Id;
			    }
			    
			    return album;
		    })
		    .ToList();

	    return groupedResult.FirstOrDefault();
    }
    
    public async Task<List<AlbumID3>> GetStarredAlbumsAsync(Guid userId)
    {
        string query = @"SELECT 
						 	al.AlbumId as Id,
						 	al.Title as Name,
						 	'' as version,
						 	a.Name as Artist,
						 	'album_' || al.AlbumId as CoverArt,
 							a.artistid AS ArtistId,
							al_sum.file_creationtime as Created,
							al_sum.Duration,
							al_sum.SongCount,
							al_sum.Year,
							al_sum.LastPlayDate as Played,
							al_sum.AlbumPlaycount as PlayCount,
 							album_rated.Rating as UserRating,
 							(case when album_rated.Starred = true 
 							    then album_rated.StarredAt 
 							    else null 
 							 end) as Starred
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
 						 join sonicserver_album_rated album_rated on album_rated.AlbumId = al.AlbumId and album_rated.UserId = @userId and album_rated.Starred = true
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
						         where UserId = @userId and Scrobble = true
						         group by TrackId
						     ) hist on hist.TrackId = m.MetadataId
						     where m.albumid = al.albumid
						     ) as al_sum on true";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = (await conn.QueryAsync<AlbumID3>(query, 
	        param: new
	        {
		        userId
	        })).ToList();

        foreach (var result in results)
        {
	        result.Artists = [new NameIdEntity(result.ArtistId, result.Artist)];
        }

        return results;
    }
}