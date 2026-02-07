using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class TrackRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    
    public TrackRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<List<TrackID3>> GetSimilarTracksAsync(Guid id, int count, ID3Type type)
    {
        string query = @"SELECT 
							sim_m.MetadataId as TrackId,
 							sim_m.AlbumId as Parent,
 							sim_m.AlbumId as AlbumId,
 							sim_m.artistid AS ArtistId,
						 	'album_' || sim_m.MetadataId as CoverArt,
 							sim_m.Title as Title,
 							sim_m.AlbumTitle as Album,
 							sim_m.ArtistName as Artist,
 							sim_m.Tag_Track as TrackNumber,
 							sim_m.Tag_Year as Year,
 							sim_m.computed_genre as Genre,
 							9999 as Size,
 							case 
 								when sim_m.Path ilike '%.mp3' then 'audio/mpeg'
 								when sim_m.Path ilike '%.m4a' then 'audio/mp4'
 								when sim_m.Path ilike '%.mp4' then 'audio/mp4'
 								when sim_m.Path ilike '%.flac' then 'audio/flac'
 								when sim_m.Path ilike '%.ogg' then 'audio/ogg'
 								when sim_m.Path ilike '%.opus' then 'audio/ogg'
 								when sim_m.Path ilike '%.wav' then 'audio/wav'
 								else 'application/octet-stream'
 							end as ContentType,
 							regexp_substr(sim_m.Path, '([a-zA-Z0-9]{2,5})$') as Suffix,
							sim_m.tag_isrc as Isrc_Single,
							sim_m.Path as Path,
							'music' AS Type,
							'song' AS MediaType,
 							    
 							EXTRACT(EPOCH FROM
							    (CASE WHEN length(m.Tag_Length) = 5 THEN '00:' || m.Tag_Length 
							    ELSE m.Tag_Length END)::interval) AS Duration,
 							    
							sim_m.file_creationtime as Created,
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
							regexp_substr(t.tags->>'replaygain_album_peak', '-?[0-9]+(\.[0-9]+)?') as AlbumPeak
						 	
						  FROM artists a
						  JOIN albums al ON al.artistid = a.artistid
						  JOIN metadata m on m.albumid = al.albumid
 							    
						  --find track in tidal data
						  join tidal_artist ta on lower(ta.name) = lower(a.name)
						  join tidal_album ta2 on ta2.artistid = ta.artistid and lower(ta2.title) = lower(al.title)
						  join tidal_track tt on tt.albumid = ta2.albumid and similarity((tt.title || ' ' || tt.version), m.title) > 0.90
						  join tidal_track_similar tts on tts.trackid = tt.trackid 
						  --get similar tracks data
						  join tidal_track tsim_tt on tsim_tt.trackid = tts.similartrackid
						  join tidal_album tsim_ta2 on tsim_ta2.albumid = tsim_tt.albumid
						  join tidal_artist tsim_ta on tsim_ta.artistid = tsim_ta2.artistid
						  --find songs we have
						  join lateral (
  							select 	sim_m.MetadataId, 
  									sim_m.path,
		  							sim_m.Title, 
		  							sim_m.Tag_Track, 
		  							sim_m.Tag_Year, 
		  							sim_m.Tag_Isrc, 
		  							sim_m.computed_genre,
		  							sim_m.File_CreationTime,
		  							sim_m.tag_alljsontags,
		  							sim_al.AlbumId,
		  							sim_al.Title as AlbumTitle,
		  							sim_ta.ArtistId,
		  							sim_ta.Name as ArtistName
	  							from metadata sim_m
	  							join artists sim_ta on lower(sim_ta.name) = lower(tsim_ta.name)
	  							JOIN albums sim_al ON sim_al.artistid = sim_ta.artistid
	  							where sim_m.albumid = sim_al.albumid 
	  							and similarity((tsim_tt.title || ' ' || tsim_tt.version), sim_m.title) > 0.90
	  							limit 1
						  ) sim_m on true
						  
 						  left join sonicserver_track_rated track_rated on track_rated.TrackId = sim_m.MetadataId
 						  LEFT JOIN LATERAL (
						    SELECT jsonb_object_agg(lower(key), value) AS tags
						    FROM jsonb_each_text(sim_m.tag_alljsontags)
						  ) t ON TRUE
						  
 						 where case 
 							    when @type = 'Artist' then a.ArtistId = @id
 							    when @type = 'AlbumId' then al.AlbumId = @id
 							    when @type = 'Track' then m.MetaDataId = @id 
 						 end
                         limit @count";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = (await conn.QueryAsync<TrackID3>(query, 
	        param: new
	        {
		        id,
		        count,
		        type = type.ToString()
	        })).ToList();

        foreach (var result in results)
        {
	        result.Artists = [new NameIdEntity(result.ArtistId, result.Artist)];
        }

        return results;
    }
    
    
    public async Task<List<GenreCountModel>> GetAllGenresAsync()
    {
        string query = @"SELECT
						     TRIM(t.genre) AS Genre,
						     COUNT(*) AS SongCount,
    						 COUNT(DISTINCT m.albumid) AS AlbumCount
						 FROM metadata m
						 JOIN LATERAL unnest(string_to_array(m.computed_genre, ';')) AS t(genre) ON TRUE
						 WHERE m.computed_genre is not null
						 GROUP BY TRIM(t.genre)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        

        return (await conn
		        .QueryAsync<GenreCountModel>(query))
				.ToList();
    }
    
    
    public async Task<TrackID3?> GetTrackByIdAsync(Guid trackId)
    {
	    string query = @"SELECT 
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
						 where m.MetadataId = @trackId";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    var results = await conn.QueryAsync<TrackID3, ReplayGain, ArtistID3, TrackID3>(query,
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
			    trackId
		    });
	    
	    var groupedResult = results
		    .GroupBy(track => track.TrackId)
		    .Select(group =>
		    {
			    var tracks = group.First();
			    tracks.Artists = group.SelectMany(track => track.Artists).ToList();
			    tracks.Isrc = group.SelectMany(track => track.Isrc).ToList();
			    return tracks;
		    })
		    .ToList();

	    return groupedResult.FirstOrDefault();
    }
    
    public async Task<List<TrackID3>> GetStarredTracksAsync(Guid userId)
    {
	    string query = @"SELECT 
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
 						 join sonicserver_track_rated track_rated on track_rated.TrackId = m.MetadataId and track_rated.UserId = @userId and track_rated.Starred = true
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
						  ) t ON TRUE";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    var results = await conn.QueryAsync<TrackID3, ReplayGain, ArtistID3, TrackID3>(query,
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
			    userId
		    });
	    
	    var groupedResult = results
		    .GroupBy(track => track.TrackId)
		    .Select(group =>
		    {
			    var tracks = group.First();
			    tracks.Artists = group.SelectMany(track => track.Artists).ToList();
			    tracks.Isrc = group.SelectMany(track => track.Isrc).ToList();
			    return tracks;
		    })
		    .ToList();

	    return groupedResult.ToList();
    }
    
    public async Task<Guid?> GetTrackIdByPathAsync(string path)
    {
	    string query = @"SELECT m.MetadataId
						 FROM metadata m
						 where m.Path = @path
						 limit 1";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return await conn.QueryFirstOrDefaultAsync<Guid>(query, 
		    new
		    {
			    path = path
		    });
    }
}