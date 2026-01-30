using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class RatingRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public RatingRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task RateTrackAsync(Guid userId, Guid trackId, int rating)
    {
	    string query = @"INSERT INTO sonicserver_track_rated (UserId, TrackId, Rating, Starred, Artist, AlbumArtist, Artists, Album, Title, ISRC, CreatedAt, UpdatedAt)
						 SELECT 
							@userId,
							@trackId,
							@rating,
							false,
						    t.tags->>'artist',
						    COALESCE(t.tags->>'albumartist', t.tags->>'album_artist'),
						    t.tags->>'artists',
						    t.tags->>'album',
						    m.Title,
						    t.tags->>'isrc',
						 	current_timestamp,
						 	current_timestamp
						 FROM metadata m
						 LEFT JOIN LATERAL (
						    SELECT jsonb_object_agg(lower(key), value) AS tags
						    FROM jsonb_each_text(m.tag_alljsontags)
						  ) t ON TRUE
						 WHERE m.MetadataId = @trackId
						 ON CONFLICT (UserId, TrackId)
						 DO UPDATE SET
						 	Rating = EXCLUDED.Rating,
						 	UpdatedAt = current_timestamp";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    userId,
			    trackId,
			    rating
		    });
    }
    
    public async Task StarTrackAsync(Guid userId, Guid trackId, bool star)
    {
	    string query = @"INSERT INTO sonicserver_track_rated (UserId, TrackId, Rating, Starred, Artist, AlbumArtist, Artists, Album, Title, ISRC, CreatedAt, UpdatedAt)
						 SELECT 
							@userId,
							@trackId,
							0,
							@star,
						    t.tags->>'artist',
						    COALESCE(t.tags->>'albumartist', t.tags->>'album_artist'),
						    t.tags->>'artists',
						    t.tags->>'album',
						    m.Title,
						    t.tags->>'isrc',
						 	current_timestamp,
						 	current_timestamp
						 FROM metadata m
						 LEFT JOIN LATERAL (
						    SELECT jsonb_object_agg(lower(key), value) AS tags
						    FROM jsonb_each_text(m.tag_alljsontags)
						  ) t ON TRUE
						 WHERE m.MetadataId = @trackId
						 ON CONFLICT (UserId, TrackId)
						 DO UPDATE SET
						 	Starred = EXCLUDED.Starred,
						 	UpdatedAt = current_timestamp";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    userId,
			    trackId,
			    star
		    });
    }
    
    public async Task RateArtistAsync(Guid userId, Guid artistId, int rating)
    {
	    string query = @"INSERT INTO sonicserver_artist_rated (UserId, ArtistId, Rating, Starred, Artist, CreatedAt, UpdatedAt)
						 SELECT 
							@userId,
							@artistId,
							@rating,
							false,
						    a.Name,
						 	current_timestamp,
						 	current_timestamp
						 FROM artists a
						 WHERE a.ArtistId = @artistId
						 ON CONFLICT (UserId, ArtistId)
						 DO UPDATE SET
						 	Rating = EXCLUDED.Rating,
						 	UpdatedAt = current_timestamp";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    userId,
			    artistId,
			    rating
		    });
    }
    
    public async Task StarArtistAsync(Guid userId, Guid artistId, bool star)
    {
	    string query = @"INSERT INTO sonicserver_artist_rated (UserId, ArtistId, Rating, Starred, Artist, CreatedAt, UpdatedAt)
						 SELECT 
							@userId,
							@artistId,
							0,
							@star,
						    a.Name,
						 	current_timestamp,
						 	current_timestamp
						 FROM artists a
						 WHERE a.ArtistId = @artistId
						 ON CONFLICT (UserId, ArtistId)
						 DO UPDATE SET
						 	Starred = EXCLUDED.Starred,
						 	UpdatedAt = current_timestamp";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    userId,
			    artistId,
			    star
		    });
    }
    
    public async Task RateAlbumAsync(Guid userId, Guid albumId, int rating)
    {
	    string query = @"INSERT INTO sonicserver_album_rated (UserId, AlbumId, Rating, Starred, Artist, Album, CreatedAt, UpdatedAt)
						 SELECT 
							@userId,
							@albumId,
							@rating,
							false,
						    artist.Name,
						    album.Title,
						 	current_timestamp,
						 	current_timestamp
						 FROM albums album
						 join artists artist on artist.artistid = album.artistid
						 WHERE album.AlbumId = @albumId
						 ON CONFLICT (UserId, AlbumId)
						 DO UPDATE SET
						 	Rating = EXCLUDED.Rating,
						 	UpdatedAt = current_timestamp";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    userId,
			    albumId,
			    rating
		    });
    }
    
    public async Task StarAlbumAsync(Guid userId, Guid albumId, bool star)
    {
	    string query = @"INSERT INTO sonicserver_album_rated (UserId, AlbumId, Rating, Starred, Artist, Album, CreatedAt, UpdatedAt)
						 SELECT 
							@userId,
							@albumId,
							0,
							@star,
						    artist.Name,
						    album.Title,
						 	current_timestamp,
						 	current_timestamp
						 FROM albums album
						 join artists artist on artist.artistid = album.artistid
						 WHERE album.AlbumId = @albumId
						 ON CONFLICT (UserId, AlbumId)
						 DO UPDATE SET
						 	Starred = EXCLUDED.Starred,
						 	UpdatedAt = current_timestamp";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    userId,
			    albumId,
			    star
		    });
    }
}