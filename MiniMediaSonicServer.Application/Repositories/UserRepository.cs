using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
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
                         where su.username = @username
                         limit 1";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        return await conn.QueryFirstOrDefaultAsync<UserModel>(query,
                param: new
                {
                    username
                });
    }
}