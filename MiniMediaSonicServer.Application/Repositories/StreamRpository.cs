using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class StreamRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public StreamRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<string?> GetTrackPathByIdAsync(Guid trackId)
    {
	    string query = @"SELECT m.path
						 FROM metadata m
						 where m.metadataid = @trackId
						 limit 1";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return await conn.ExecuteScalarAsync<string>(query,
		    param: new
		    {
			    trackId
		    });
    }
}