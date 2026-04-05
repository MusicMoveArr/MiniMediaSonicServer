using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class BookmarkRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public BookmarkRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task UpsertBookmarkAsync(
        Guid userId, 
        Guid trackId, 
        long position, 
        string comment)
    {
        string query = @"
            INSERT INTO sonicserver_user_bookmark_track (UserId, TrackId, Position, Comment, CreatedAt, UpdatedAt)
            VALUES (@userId, @trackId, @position, @comment, current_timestamp, current_timestamp)
            ON CONFLICT (UserId, TrackId)
            DO UPDATE SET
			    Position = EXCLUDED.Position,
			    Comment = EXCLUDED.Comment,
			    UpdatedAt = current_timestamp";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn
            .ExecuteAsync(query, param: new
            {
                userId, 
                trackId, 
                position,
                comment = comment ?? string.Empty
            });
    }
    
    public async Task<List<TrackBookmarkModel>> GetBookmarksByUserIdAsync(Guid userId)
    {
        string query = @"
            SELECT track.*, u.Username 
            FROM sonicserver_user_bookmark_track track
            join sonicserver_user u on u.UserId = track.UserId and u.IsDeleted = false
            WHERE track.UserId = @userId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        return (await conn
            .QueryAsync<TrackBookmarkModel>(query, param: new
            {
                userId
            }))
            .ToList();
    }
    
    public async Task DeleteBookmarkAsync(Guid userId, Guid trackId)
    {
        string query = @"
            DELETE FROM sonicserver_user_bookmark_track
            WHERE UserId = @userId and TrackId = @trackId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn
            .ExecuteAsync(query, param: new
            {
                userId,
                trackId
            });
    }
}