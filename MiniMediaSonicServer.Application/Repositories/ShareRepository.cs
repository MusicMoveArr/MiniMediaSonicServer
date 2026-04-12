using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.Database;
using Npgsql;

namespace MiniMediaSonicServer.Application.Repositories;

public class ShareRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    
    public ShareRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<List<ShareModel>> GetAllSharesAsync(Guid userId)
    {
	    string query = @"select share.*, u.Username
						 from sonicserver_user_share share
						 join sonicserver_user u on u.UserId = share.UserId and u.IsDeleted = false
						 where share.UserId = @userId
						 and share.IsDeleted = false";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return (await conn.QueryAsync<ShareModel>(query, 
		    param: new
		    {
			    userId
		    })).ToList();
    }
    
    public async Task<ShareModel?> GetShareAsync(string shareName)
    {
	    string query = @"select *
						 from sonicserver_user_share
						 where shareName = @shareName
						 and IsDeleted = false";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    return await conn.QueryFirstOrDefaultAsync<ShareModel>(query, 
		    param: new
		    {
			    shareName
		    });
    }
    
    public async Task CreateShareAsync(
	    Guid userId, 
	    string shareName, 
	    string? description, 
	    DateTime? expiresAt, 
	    string type, 
	    Guid mediaId)
    {
        string query = @"INSERT INTO sonicserver_user_share (ShareId, UserId, ShareName, Description, 
                                      						 ExpiresAt, Type, MediaId,
                                    						 CreatedAt, UpdatedAt)
						 VALUES(@shareId, @userId, @shareName, @description, 
                                @expiresAt, @type, @mediaId,
                                @createdAt, @updatedAt)
						 ON CONFLICT (ShareId, UserId, ShareName)
						 DO UPDATE SET
						 	Description = EXCLUDED.Description,
						 	UpdatedAt = current_timestamp";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        await conn.ExecuteAsync(query, 
            param: new
            {
				shareId = Guid.NewGuid(), 
				userId, 
				shareName, 
				description = description ?? string.Empty, 
				expiresAt, 
				type, 
				mediaId,
				createdAt = DateTime.Now,
				updatedAt = DateTime.Now
            });
    }
    
    public async Task DeleteShareAsync(Guid userId, Guid shareId)
    {
	    string query = @"UPDATE sonicserver_user_share 
						 SET DeletedAt = @deletedAt, 
						     IsDeleted = true
						 where shareId = @shareId 
						   and userId = @userId
						   and IsDeleted = false";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    shareId, 
			    userId,
			    deletedAt = DateTime.Now
		    });
    }
    
    public async Task IncrementVisitorCountAsync(Guid shareId)
    {
	    string query = @"UPDATE sonicserver_user_share
						 SET VisitCount = VisitCount + 1, 
						     LastVisitedAt = @lastVisitedAt
						 where shareId = @shareId 
						   and IsDeleted = false";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    shareId,
			    lastVisitedAt = DateTime.Now
		    });
    }
    
    public async Task UpdateShareAsync(Guid shareId, Guid userId, string? description, DateTime? expiresAt)
    {
	    string query = @"UPDATE sonicserver_user_share 
						 SET Description = @description, 
						     ExpiresAt = @expiresAt,
						     UpdatedAt = @updatedAt
						 where shareId = @shareId and UserId = @userId
						   and IsDeleted = false";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

	    await conn.ExecuteAsync(query, 
		    param: new
		    {
			    shareId,
			    userId,
			    description,
			    expiresAt,
			    updatedAt = DateTime.Now
		    });
    }
}