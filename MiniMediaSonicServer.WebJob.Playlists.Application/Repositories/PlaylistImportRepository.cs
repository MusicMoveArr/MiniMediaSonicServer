using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.WebJob.Playlists.Application.Models.Database;
using Npgsql;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Repositories;

public class PlaylistImportRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    
    public PlaylistImportRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<PlaylistImportModel?> GetImportedPlaylistByPathAsync(string path)
    {
        string query = @"SELECT *
						 FROM sonicserver_playlist_import
						 where Path = @path
						 limit 1";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();

        return await conn.QueryFirstOrDefaultAsync<PlaylistImportModel>(query,
            param: new
            {
                path
            });
    }
    
    public async Task<Guid> InsertPlaylistImportAsync(string path, bool isGlobal, string name, DateTime fileModifiedDate)
    {
        string query = @"INSERT INTO sonicserver_playlist_import(ImportId, Path, IsGlobal, Name, FileModifiedDate, CreatedAt, UpdatedAt)
						 VALUES(@importId, @path, @isGlobal, @name, @fileModifiedDate, current_timestamp, current_timestamp)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();

        Guid importId = Guid.NewGuid();
        await conn.ExecuteAsync(query,
            param: new
            {
                importId,
                path,
                isGlobal, 
                name,
                fileModifiedDate
            });

        return importId;
    }
    
    public async Task<PlaylistImportUserModel?> GetPlaylistImportUserAsync(Guid importId, Guid userId)
    {
        string query = @"select * 
                         from sonicserver_playlist_import_user
                         where ImportId = @importId
                         and UserId = @userId
                         limit 1";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();

        return await conn.QueryFirstOrDefaultAsync<PlaylistImportUserModel>(query, 
            param: new
            {
                importId,
                userId
            });
    }
    
    public async Task InsertPlaylistImportUserAsync(Guid importId, Guid userId, Guid playlistId)
    {
        string query = @"INSERT INTO sonicserver_playlist_import_user(ImportId, UserId, PlaylistId, CreatedAt, UpdatedAt)
						 VALUES(@importId, @userId, @PlaylistId, current_timestamp, current_timestamp)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();

        await conn.ExecuteAsync(query,
            param: new
            {
                importId,
                userId,
                playlistId
            });
    }
}