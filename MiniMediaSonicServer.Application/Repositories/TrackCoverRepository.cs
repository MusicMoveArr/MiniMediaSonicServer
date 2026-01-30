using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class TrackCoverRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public TrackCoverRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<List<string>> GetTrackPathByAlbumIdAsync(Guid albumId)
    {
        string query = @"SELECT m.path
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
						 JOIN metadata m on m.albumid = al.albumid
						 where al.AlbumId = @albumId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        return (await conn.QueryAsync<string>(query,
            param: new
            {
                albumId
            })).ToList();
    }
    
    public async Task<List<string>> GetTrackPathByArtistIdAsync(Guid artistId)
    {
        string query = @"SELECT m.path
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
						 JOIN metadata m on m.albumid = al.albumid
						 where a.ArtistId = @artistId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        return (await conn.QueryAsync<string>(query,
            param: new
            {
                artistId
            })).ToList();
    }
    
    public async Task<List<string>> GetTrackPathByPlaylistIdAsync(Guid playlistId)
    {
        string query = @"SELECT m.path
						 FROM sonicserver_playlist_track track
						 JOIN metadata m on m.MetadataId = track.TrackId
						 where track.PlaylistId = @playlistId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        return (await conn.QueryAsync<string>(query,
            param: new
            {
                playlistId
            })).ToList();
    }
}