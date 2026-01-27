using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
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
    
    public async Task<List<TrackID3>> GetSimilarTracksAsync(Guid trackId, int count)
    {
        string query = @"SELECT distinct on (sim_ta.name, sim_m.title) 
							sim_m.MetadataId as TrackId,
 							sim_al.AlbumId as Parent,
 							sim_al.AlbumId as AlbumId,
 							sim_ta.artistid AS ArtistId,
						 	'album_' || sim_m.MetadataId as CoverArt,
 							sim_m.Title as Title,
 							sim_al.Title as Album,
 							sim_ta.Name as Artist,
 							sim_m.Tag_Track as TrackNumber,
 							sim_m.Tag_Year as Year,
 							'' as Genre,
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
 							    
 							FLOOR(EXTRACT(EPOCH FROM
							    CASE
						          WHEN sim_m.Tag_Length !~ ':' THEN NULL 
						          WHEN sim_m.Tag_Length ~ '^\d{{1,2}}:\d{{2}}$' THEN ('0:' || sim_m.Tag_Length)::interval
						          ELSE sim_m.Tag_Length::interval
						        END) /100) AS Duration,
 							    
							sim_m.file_creationtime as Created,
							t.tags->>'bitrate' AS BitRate,
							16 as BitDepth,
							44100 as SamplingRate,
							2 as ChannelCount,
							regexp_substr(t.tags->>'bpm', '[0-9]*') as BPM,
							regexp_substr(t.tags->>'replaygain_track_gain', '[0-9\-\.]*') as TrackGain,
							regexp_substr(t.tags->>'replaygain_album_gain', '[0-9\-\.]*') as AlbumGain,
							regexp_substr(t.tags->>'replaygain_track_peak', '[0-9\-\.]*') as TrackPeak,
							regexp_substr(t.tags->>'replaygain_album_peak', '[0-9\-\.]*') as AlbumPeak
						 	
						  FROM artists a
						  JOIN albums al ON al.artistid = a.artistid
						  JOIN metadata m on m.albumid = al.albumid
						  --find track in tidal data
						  join tidal_artist ta on ta.name = a.name 
						  join tidal_album ta2 on ta2.artistid = ta.artistid and ta2.title = al.title 
						  join tidal_track tt on tt.albumid = ta2.albumid and similarity((tt.title || ' ' || tt.version), m.title) > 0.90
						  join tidal_track_similar tts on tts.trackid = tt.trackid 
						  --get similar tracks data
						  join tidal_track tsim_tt on tsim_tt.trackid = tts.similartrackid
						  join tidal_album tsim_ta2 on tsim_ta2.albumid = tsim_tt.albumid
						  join tidal_artist tsim_ta on tsim_ta.artistid = tsim_ta2.artistid
						  --find songs we have
						  join artists sim_ta on sim_ta.name = tsim_ta.name
						  JOIN albums sim_al ON sim_al.artistid = sim_ta.artistid
						  JOIN metadata sim_m on sim_m.albumid = sim_al.albumid and similarity((tsim_tt.title || ' ' || tsim_tt.version), sim_m.title) > 0.90
						  
 						  LEFT JOIN LATERAL (
						    SELECT jsonb_object_agg(lower(key), value) AS tags
						    FROM jsonb_each_text(sim_m.tag_alljsontags)
						  ) t ON TRUE
						  
						 where m.MetaDataId = @trackId
                         limit @count";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = (await conn.QueryAsync<TrackID3>(query, 
	        param: new
	        {
		        trackId,
		        count
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
						 JOIN LATERAL unnest(string_to_array(m.genre_list, ';')) AS t(genre) ON TRUE
						 WHERE m.genre_list is not null
						 GROUP BY TRIM(t.genre)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        

        return (await conn
		        .QueryAsync<GenreCountModel>(query))
				.ToList();
    }
    
    
}