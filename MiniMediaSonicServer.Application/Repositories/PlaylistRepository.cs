using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class PlaylistRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    
    public PlaylistRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<List<PlaylistModel>> GetAllPlaylistsAsync(Guid userId)
    {
        string query = @"SELECT distinct on (playlist.PlaylistId)
    						playlist.PlaylistId,
    						playlist.UserId,
    						playlist.Name,
    						playlist.Public,
    						playlist.CreatedAt,
    						playlist.UpdatedAt,
    						track.SongCount as SongCount,
    						1 as TotalDuration
						 from sonicserver_playlist playlist
						 left join lateral (
						     select count(track.TrackId) as SongCount 
							 from sonicserver_playlist_track track 
							 where track.PlaylistId = playlist.PlaylistId)
							 track on true
						 where userid = @userId
						 	   and playlist.IsDeleted = false";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        return (await conn.QueryAsync<PlaylistModel>(query, 
	        param: new
	        {
		        userId
	        })).ToList();
    }
    
    public async Task<PlaylistModel> GetPlaylistByIdAsync(Guid playlistId)
    {
	    string query = @"SELECT
    						playlist.PlaylistId, 
    						playlist.UserId, 
    						playlist.Name, 
    						playlist.Public, 
    						playlist.CreatedAt, 
    						playlist.UpdatedAt,
    
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
 						 from sonicserver_playlist playlist
 						 left join sonicserver_playlist_track playlist_track on playlist_track.playlistid = playlist.playlistid
 					     left JOIN metadata m on m.MetadataId = playlist_track.TrackId
 						 left JOIN albums al ON al.AlbumId = m.AlbumId
						 left JOIN artists a ON a.ArtistId = al.ArtistId
						 
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
						 where playlist.PlaylistId = @playlistId
						 	   and playlist.IsDeleted = false";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    var results = await conn.QueryAsync<PlaylistModel, TrackID3, ReplayGain, ArtistID3, PlaylistModel>(query,
		    (playlist, track, replayGain, extraArtist) =>
		    {
			    if (track != null)
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
			    
				    playlist.Tracks.Add(track);
			    }
			    
			    return playlist;
		    },
		    splitOn: "TrackId, TrackGain, Id",
		    param: new
		    {
			    playlistId
		    });
	    
	    var groupedResult = results
		    .GroupBy(playlist => playlist.PlaylistId)
		    .Select(group =>
		    {
			    var playlist = group.First();
			    playlist.Tracks = group.SelectMany(playlist => playlist.Tracks).ToList();
			    playlist.SongCount = playlist.Tracks.Count;
			    return playlist;
		    })
		    .ToList();

	    return groupedResult.FirstOrDefault();
    }
    
    public async Task<Guid> CreatePlaylistAsync(Guid userId, string name)
    {
	    Guid playlistId = Guid.NewGuid();
	    string query = @"INSERT INTO sonicserver_playlist (PlaylistId, UserId, Name, Public)
						 VALUES (@playlistId, @userId, @name, true)";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query,
		    param: new
		    {
			    playlistId,
			    userId,
			    name
		    });
	    return playlistId;
    }
    
    public async Task AddTrackToPlaylistAsync(Guid playlistId, Guid trackId)
    {
	    string query = @"INSERT INTO sonicserver_playlist_track (PlayListId, TrackId, TrackOrder, CreatedAt, Artist, AlbumArtist, Artists, Album, Title, ISRC)
						 SELECT 
							@playlistId,
							@trackId,
							COALESCE((select max(t.TrackOrder)+1 from sonicserver_playlist_track t where t.PlaylistId = @playlistId), 1),
						 	current_timestamp,
						    t.tags->>'artist',
						    COALESCE(t.tags->>'albumartist', t.tags->>'album_artist'),
						    t.tags->>'artists',
						    t.tags->>'album',
						    m.Title,
						    t.tags->>'isrc'
						 FROM metadata m
						 LEFT JOIN LATERAL (
						    SELECT jsonb_object_agg(lower(key), value) AS tags
						    FROM jsonb_each_text(m.tag_alljsontags)
						  ) t ON TRUE
						 WHERE m.MetadataId = @trackId
						 ON CONFLICT (PlayListId, TrackId)
						 DO NOTHING";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    playlistId,
			    trackId
		    });
    }
    
    public async Task UpdatePlaylistUpdatedAtAsync(Guid playlistId)
    {
	    string query = @"UPDATE sonicserver_playlist SET UpdatedAt = current_timestamp where PlaylistId = @playlistId;";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    playlistId
		    });
    }
    
    public async Task SetPlaylistDeletedAsync(Guid playlistId)
    {
	    string query = @"UPDATE sonicserver_playlist 
						 SET UpdatedAt = current_timestamp,
						     IsDeleted = true,
						     DeletedAt = current_timestamp
                         where PlaylistId = @playlistId 
                           	   and IsDeleted = false";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    playlistId
		    });
    }
    
    public async Task<bool> PlaylistExistsAsync(Guid playlistId)
    {
	    string query = @"SELECT true
						 from sonicserver_playlist playlist
						 where playlist.PlaylistId = @playlistId
						 	   and playlist.IsDeleted = false";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return await conn.QueryFirstOrDefaultAsync<bool>(query, 
		    param: new
		    {
			    playlistId
		    });
    }
}