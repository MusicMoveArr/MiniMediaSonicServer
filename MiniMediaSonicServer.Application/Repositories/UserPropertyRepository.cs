using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class UserPropertyRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public UserPropertyRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<string?> GetUserPropertyAsync(Guid userId, string name)
    {
        string query = @"SELECT Value
                         FROM sonicserver_user_property sup
                         where sup.UserId = @userId
                               and sup.Name = @name
                         limit 1";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        return await conn.ExecuteScalarAsync<string>(query,
                param: new
                {
                    userId,
                    name
                });
    }
    
    public async Task SetUserPropertyAsync(Guid userId, string name, string value)
    {
        string query = @"UPDATE sonicserver_user_property
                         SET Value = @value,
                             UpdatedAt = current_timestamp
                         where UserId = @userId
                               and Name = @name";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        await conn.ExecuteAsync(query, param: new
        {
            userId,
            name,
            value
        });
    }
}