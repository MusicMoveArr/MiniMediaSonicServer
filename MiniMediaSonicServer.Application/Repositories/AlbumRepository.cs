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
						     JOIN metadata m
						         ON m.MetadataId = playhistory.TrackId
						     WHERE playhistory.UserId = @userId
						     GROUP BY m.AlbumId
						 )
						 SELECT 
						 	al.AlbumId as Id,
						 	al.Title as Name,
						 	'' as version,
						 	a.Name as Artist,
						 	m.tag_year as year,
						 	'album_' || al.AlbumId as CoverArt,
 							a.artistid AS ArtistId,
							m.file_creationtime as Created,
							COALESCE(playhistory.AlbumPlaycount, 0) as PlayCount,
 							album_rated.Rating as UserRating,
 							(case when album_rated.Starred = true 
 							    then album_rated.UpdatedAt 
 							    else null 
 							 end) as Starred
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
 						 left join sonicserver_album_rated album_rated on 
						     album_rated.AlbumId = al.AlbumId 
						     and album_rated.UserId = @userId
						     
						 JOIN lateral (
						     select * from metadata m 
						     where m.albumid = al.albumid 
						     order by m.tag_year 
						     desc limit 1) as m on true
						     
						 JOIN lateral (
						     select m.file_creationtime as file_creationtime 
						     from metadata m 
						     where m.albumid = al.albumid 
						     order by m.file_creationtime desc
						     limit 1) as recent_m on true
						                
						 LEFT JOIN album_playhistory playhistory
							ON playhistory.AlbumId = al.AlbumId
						 
						where (CASE 
					             WHEN @type = 'byGenre' THEN 
									m.computed_genre ILIKE '%' || @genre || '%'
					             WHEN @type in ('recent', 'frequent') THEN 
									playhistory.AlbumId is not null
					             ELSE al.record_id >= @offset and al.record_id <= @offset + @limit
					         END)
 						
						ORDER BY
				         CASE 
				             WHEN @type = 'frequent' THEN playhistory.AlbumPlaycount
				             WHEN @type = 'random' THEN random()
				             WHEN @type = 'byYear' THEN m.tag_year
				             WHEN @type = 'starred' THEN 0
				             ELSE NULL
				         END DESC,
				         CASE 
				             WHEN @type = 'alphabeticalByName' THEN al.Title
				         END ASC, 
				         CASE 
				             WHEN @type = 'recent' THEN playhistory.UpdatedAt
				             WHEN @type = 'newest' THEN recent_m.file_creationtime
				         END DESC
                         limit @limit";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = (await conn.QueryAsync<AlbumID3>(query, 
	        param: new
	        {
		        limit = request.Size,
		        type = request.Type,
		        offset = request.Offset,
		        genre = request.Genre,
		        userId
	        })).ToList();

        foreach (var result in results)
        {
	        result.Artists = [new NameIdEntity(result.ArtistId, result.Artist)];
        }

        return results;
    }
    
    public async Task<AlbumID3> GetAlbumId3WithTracksAsync(Guid albumId)
    {
	    string query = @"SELECT 
 							al.AlbumId as Id,
 							al.Title as Name,
 							'' as version,
 							a.Name as Artist,
 							m.tag_year as Year,
						 	'album_' || al.AlbumId as CoverArt,
 							a.artistid AS ArtistId,
 							m.file_creationtime as Created,
 							album_rated.Rating as UserRating,
 							(case when album_rated.Starred = true 
 							    then album_rated.UpdatedAt 
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
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
						 JOIN metadata m on m.albumid = al.albumid
 						 left join sonicserver_track_rated track_rated on track_rated.TrackId = m.MetadataId
 						 left join sonicserver_album_rated album_rated on album_rated.AlbumId = al.AlbumId
 							    
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
						 where al.albumid = @id";

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
			    id = albumId
		    });
	    
	    var groupedResult = results
		    .GroupBy(album => album.Id)
		    .Select(group =>
		    {
			    var album = group.First();
			    album.Song = group
				    .SelectMany(album => album.Song)
				    .DistinctBy(track => track.TrackId)
				    .OrderBy(track => track.TrackNumber)
				    .ThenBy(track => track.DiscNumber)
				    .ToList();
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
						 	m.tag_year as year,
						 	'album_' || al.AlbumId as CoverArt,
 							a.artistid AS ArtistId,
							m.file_creationtime as Created,
 							album_rated.Rating as UserRating,
 							(case when album_rated.Starred = true 
 							    then album_rated.UpdatedAt 
 							    else null 
 							 end) as Starred
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
 						 join sonicserver_album_rated album_rated on album_rated.AlbumId = al.AlbumId and album_rated.UserId = @userId and album_rated.Starred = true
						 JOIN lateral (select * from metadata m where m.albumid = al.albumid order by m.tag_year desc limit 1) as m on true
						 JOIN lateral (
						     select m.file_creationtime as file_creationtime 
						     from metadata m 
						     where m.albumid = al.albumid 
						     order by m.file_creationtime desc
						     limit 1) as recent_m on true";

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