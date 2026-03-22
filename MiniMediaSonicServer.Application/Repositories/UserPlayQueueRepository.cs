using Dapper;
using DapperBulkQueries.Common;
using DapperBulkQueries.Npgsql;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class UserPlayQueueRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    private readonly List<string> _trackColumns = new()
    {
        "UserId", 
        "TrackId", 
        "Index", 
        "CreatedAt", 
        "UpdatedAt"
    };
    
    public UserPlayQueueRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task UpsertUserPlayQueueAsync(
        Guid userId, 
        Guid? currentTrackId, 
        long trackPosition, 
        string clientName,
        DateTime userDateTime)
    {
        string query = @"INSERT INTO sonicserver_user_playqueue (UserId, CurrentTrackId, TrackPosition, UpdatedByAppName, CreatedAt, UpdatedAt)
                         VALUES(@userId, @currentTrackId, @trackPosition, @clientName, @userDateTime, @userDateTime)
						 ON CONFLICT (UserId)
						 DO UPDATE SET
						 	CurrentTrackId = EXCLUDED.CurrentTrackId,
						 	TrackPosition = EXCLUDED.TrackPosition,
						 	UpdatedByAppName = EXCLUDED.UpdatedByAppName,
						 	UpdatedAt = EXCLUDED.UpdatedAt";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        await conn.ExecuteAsync(query,
                param: new
                {
                    userId,
                    currentTrackId,
                    trackPosition,
                    clientName,
                    userDateTime
                });
    }
    
    public async Task UpsertUserPlayQueueTrackAsync(
        Guid userId, 
        Guid? trackId, 
        int index, 
        DateTime userDateTime)
    {
        string query = @"INSERT INTO sonicserver_user_playqueue_track (UserId, TrackId, Index, CreatedAt, UpdatedAt)
                         VALUES(@userId, @trackId, @index, @userDateTime, @userDateTime)
						 ON CONFLICT (UserId, Index)
						 DO UPDATE SET
						 	TrackId = EXCLUDED.TrackId,
						 	Index = EXCLUDED.Index,
						 	UpdatedAt = EXCLUDED.UpdatedAt";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        await conn.ExecuteAsync(query,
            param: new
            {
                userId,
                trackId,
                index,
                userDateTime
            });
    }
    
    public async Task<List<UserPlayQueueTrackModel>> GetUserPlayQueueTracksAsync(Guid userId)
    {
        string query = @"SELECT * 
                         FROM sonicserver_user_playqueue_track
                         where UserId = @userId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        return (await conn.QueryAsync<UserPlayQueueTrackModel>(query,
            param: new
            {
                userId
            }))
            .ToList();
    }
    
    public async Task DeletePlayQueueTracksAsync(Guid userId, int startingIndex)
    {
        string query = @"DELETE FROM sonicserver_user_playqueue_track
                         where UserId = @userId
                         and index >= @startingIndex";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.ExecuteAsync(query, param: new
        {
            userId,
            startingIndex
        });
    }
    
    public async Task BulkInsertQueueTracksAsync(List<UserPlayQueueTrackModel> tracks)
    {
        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.ExecuteBulkInsertAsync(
            "sonicserver_user_playqueue_track",
            tracks,
            _trackColumns, 
            onConflict: OnConflict.DoNothing);
    }
}