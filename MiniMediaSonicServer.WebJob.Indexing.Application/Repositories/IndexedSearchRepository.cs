using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using Npgsql;

namespace MiniMediaSonicServer.WebJob.Indexing.Application.Repositories;

public class IndexedSearchRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    
    public IndexedSearchRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }

    public async Task RemoveMissingTracksAsync()
    {
        string query = @"delete from sonicserver_indexed_search s
                         where s.type = 'track'
	                         and not exists (select 1 from metadata m 
                                             where m.metadataid = s.id 
                                             limit 1)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.ExecuteAsync(query);
    }

    public async Task RemoveMissingAlbumsAsync()
    {
	    string query = @"delete from sonicserver_indexed_search s
                         where s.type = 'album'
	                         and not exists (select 1 from albums al
	                                         where al.AlbumId = s.id 
	                                         limit 1)";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.ExecuteAsync(query);
    }

    public async Task RemoveMissingArtistsAsync()
    {
	    string query = @"delete from sonicserver_indexed_search s
                         where s.type = 'artist'
	                         and not exists (select 1 from artists a
	                                         where a.ArtistId = s.id 
	                                         limit 1)";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.ExecuteAsync(query);
    }
    

    public async Task AddMissingTracks_TitleAsync()
    {
	    string query = @"insert into sonicserver_indexed_search (SearchId, type, Id, SearchTerm)
						 select gen_random_uuid(), 'track', m.MetadataId, lower(m.Title)
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
						 JOIN metadata m on m.albumid = al.albumid
						 where not exists (
						 	select 1 
						 	from sonicserver_indexed_search s
						 	where s.id = m.metadataid 
						 	and s.type = 'track'
						 	and s.SearchTerm = lower(m.Title)
						 	limit 1)";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.ExecuteAsync(query);
    }

    public async Task AddMissingTracks_ArtistTitleAsync()
    {
	    string query = @"insert into sonicserver_indexed_search (SearchId, type, Id, SearchTerm)
						 select gen_random_uuid(), 'track', m.MetadataId, lower(a.Name) || ' ' || lower(m.Title)
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
						 JOIN metadata m on m.albumid = al.albumid
						 where not exists (
						 	select 1 
						 	from sonicserver_indexed_search s
						 	where s.id = m.metadataid 
						 	and s.type = 'track'
						 	and s.SearchTerm = (lower(a.Name) || ' ' || lower(m.Title))
						 	limit 1)";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.ExecuteAsync(query);
    }

    public async Task AddMissingTracks_ArtistAlbumTitleAsync()
    {
        string query = @"insert into sonicserver_indexed_search (SearchId, type, Id, SearchTerm)
						 select gen_random_uuid(), 'track', m.MetadataId, lower(a.Name) || ' ' || lower(al.Title) || ' ' || lower(m.Title)
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
						 JOIN metadata m on m.albumid = al.albumid
						 where not exists (
						 	select 1 
						 	from sonicserver_indexed_search s
						 	where s.id = m.metadataid 
						 	and s.type = 'track'
						 	and s.SearchTerm = (lower(a.Name) || ' ' || lower(al.Title) || ' ' || lower(m.Title))
						 	limit 1)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.ExecuteAsync(query);
    }

    public async Task AddMissingAlbums_ArtistAlbumAsync()
    {
	    string query = @"insert into sonicserver_indexed_search (SearchId, type, Id, SearchTerm)
						 select gen_random_uuid(), 'album', al.AlbumId, lower(a.Name) || ' ' || lower(al.Title)
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
						 where not exists (
						 	select 1 
						 	from sonicserver_indexed_search s
						 	where s.id = al.AlbumId
						 	and s.type = 'album'
						 	and s.SearchTerm = (lower(a.Name) || ' ' || lower(al.Title))
						 	limit 1)";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.ExecuteAsync(query);
    }

    public async Task AddMissingAlbums_AlbumAsync()
    {
	    string query = @"insert into sonicserver_indexed_search (SearchId, type, Id, SearchTerm)
						 select gen_random_uuid(), 'album', al.AlbumId, lower(al.Title)
						 FROM artists a
						 JOIN albums al ON al.artistid = a.artistid
						 where not exists (
						 	select 1 
						 	from sonicserver_indexed_search s
						 	where s.id = al.AlbumId
						 	and s.type = 'album'
						 	and s.SearchTerm = lower(al.Title)
						 	limit 1)";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.ExecuteAsync(query);
    }

    public async Task AddMissingArtistsAsync()
    {
	    string query = @"insert into sonicserver_indexed_search (SearchId, type, Id, SearchTerm)
						 select gen_random_uuid(), 'artist', a.ArtistId, lower(a.Name)
						 FROM artists a
						 where not exists (
						 	select 1 
						 	from sonicserver_indexed_search s
						 	where s.id = a.ArtistId
						 	and s.type = 'artist'
						 	and s.SearchTerm = lower(a.Name)
						 	limit 1)";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.ExecuteAsync(query);
    }
}