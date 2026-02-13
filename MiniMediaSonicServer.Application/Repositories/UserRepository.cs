using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class UserRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public UserRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<UserModel?> GetUserByUsernameAsync(string username)
    {
        string query = @"SELECT *
                         FROM sonicserver_user su
                         where trim(lower(su.username)) = trim(lower(@username))
                               and IsDeleted = false
                         limit 1";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        return await conn.QueryFirstOrDefaultAsync<UserModel>(query,
                param: new
                {
                    username
                });
    }
    
    public async Task<List<UserModel>> GetAllUsersAsync()
    {
        string query = @"SELECT *
                         FROM sonicserver_user su
                         where IsDeleted = false";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        return (await conn.QueryAsync<UserModel>(query))
            .ToList();
    }
    
    public async Task<List<Guid>> GetAllUserIdsAsync()
    {
        string query = @"SELECT su.UserId
                         FROM sonicserver_user su
                         where IsDeleted = false";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        return (await conn.QueryAsync<Guid>(query))
            .ToList();
    }
    
    public async Task<bool> UserExistsByUsernameAsync(string username)
    {
        string query = @"SELECT true
                         FROM sonicserver_user su
                         where trim(lower(su.username)) = trim(lower(@username))
                               and IsDeleted = false
                         limit 1";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        return await conn.QueryFirstOrDefaultAsync<bool>(query,
            param: new
            {
                username
            });
    }
    
    public async Task<bool> UserExistsByEmailAsync(string email)
    {
        string query = @"SELECT true
                         FROM sonicserver_user su
                         where trim(lower(su.email)) = trim(lower(@email))
                               and IsDeleted = false
                         limit 1";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        return await conn.QueryFirstOrDefaultAsync<bool>(query,
            param: new
            {
                email
            });
    }
    
    public async Task UpdateUserByUsernameAsync(UpdateUserRequest request)
    {
        string query = @"UPDATE sonicserver_user
                         SET Email = COALESCE(@Email, Email),
                             LdapAuthenticated = COALESCE(@LdapAuthenticated, LdapAuthenticated),
                             AdminRole = COALESCE(@AdminRole, AdminRole),
                             SettingsRole = COALESCE(@SettingsRole, SettingsRole),
                             StreamRole = COALESCE(@StreamRole, StreamRole),
                             JukeboxRole = COALESCE(@JukeboxRole, JukeboxRole),
                             DownloadRole = COALESCE(@DownloadRole, DownloadRole),
                             UploadRole = COALESCE(@UploadRole, UploadRole),
                             CoverArtRole = COALESCE(@CoverArtRole, CoverArtRole),
                             CommentRole = COALESCE(@CommentRole, CommentRole),
                             PodcastRole = COALESCE(@PodcastRole, PodcastRole),
                             ShareRole = COALESCE(@ShareRole, ShareRole),
                             VideoConversionRole = COALESCE(@VideoConversionRole, VideoConversionRole),
                             MusicFolderId = COALESCE(@MusicFolderId, MusicFolderId),
                             MaxBitRate = COALESCE(@MaxBitRate, MaxBitRate),
                             UpdatedAt = current_timestamp
                         where trim(lower(username)) = trim(lower(@username))
                               and IsDeleted = false";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        await conn.ExecuteAsync(query, param: request);
    }
    
    
    public async Task CreateUserAsync(
        CreateUserRequest request, 
        string password, 
        string tokenBasedAuth)
    {
        string query = @"INSERT INTO sonicserver_user 
                            (UserId, 
                             Username, 
                             Password, 
                             Name, 
                             Email, 
                             CreationDateTime, 
                             TokenBasedAuth,
                             LdapAuthenticated,
                             AdminRole,
                             SettingsRole,
                             StreamRole,
                             JukeboxRole,
                             DownloadRole,
                             UploadRole,
                             CoverArtRole,
                             CommentRole,
                             PodcastRole,
                             ShareRole,
                             VideoConversionRole,
                             MusicFolderId,
                             MaxBitRate)
                         VALUES(@userId, @Username, @Password,  @Name,  @Email, current_timestamp, 
                                @tokenBasedAuth,
                                COALESCE(@LdapAuthenticated, true),
                                COALESCE(@AdminRole, true),
                                COALESCE(@SettingsRole, true),
                                COALESCE(@StreamRole, true),
                                COALESCE(@JukeboxRole, true),
                                COALESCE(@DownloadRole, true),
                                COALESCE(@UploadRole, true),
                                COALESCE(@CoverArtRole, true),
                                COALESCE(@CommentRole, true), 
                                COALESCE(@PodcastRole, true),
                                COALESCE(@ShareRole, true),
                                COALESCE(@VideoConversionRole, true),
                                COALESCE(@MusicFolderId, 1),
                                COALESCE(@MaxBitRate, 0))";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        await conn.ExecuteAsync(query, param: new
        {
            UserId = Guid.NewGuid(),
            request.Username,
            password,
            Name = request.Username,
            request.Email,
            tokenBasedAuth,
            request.LdapAuthenticated,
            request.AdminRole,
            request.SettingsRole,
            request.StreamRole,
            request.JukeboxRole,
            request.DownloadRole,
            request.UploadRole,
            request.CoverArtRole,
            request.CommentRole,
            request.PodcastRole,
            request.ShareRole,
            request.VideoConversionRole,
            request.MusicFolderId,
            request.MaxBitRate
            
        });
    }
    
    public async Task UpdatePasswordByUsernameAsync(string username, string password, string tokenBasedAuth)
    {
        string query = @"UPDATE sonicserver_user
                         SET password = @password,
                             tokenBasedAuth = @tokenBasedAuth,
                             UpdatedAt = current_timestamp
                         where trim(lower(username)) = trim(lower(@username))
                               and IsDeleted = false";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        await conn.ExecuteAsync(query, param: new
        {
            username,
            password,
            tokenBasedAuth
        });
    }
    
    public async Task SetUserDeletedByUsernameAsync(string username)
    {
        string query = @"UPDATE sonicserver_user
						 SET UpdatedAt = current_timestamp,
						     IsDeleted = true,
						     DeletedAt = current_timestamp
                         where trim(lower(Username)) = trim(lower(@username))
                           	   and IsDeleted = false";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        await conn.ExecuteAsync(query, 
            param: new
            {
                username
            });
    }
    
    public async Task<TimeZoneInfo?> GetTimezoneByUserIdAsync(Guid userId)
    {
        string query = @"SELECT Timezone 
                         from sonicserver_user
                         where UserId = @userId
                           	   and IsDeleted = false";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        string? timezoneName = await conn.ExecuteScalarAsync<string>(query, 
            param: new
            {
                userId
            });

        if (string.IsNullOrWhiteSpace(timezoneName))
        {
            return null;
        }
        return TimeZoneInfo.FindSystemTimeZoneById(timezoneName);
    }
}