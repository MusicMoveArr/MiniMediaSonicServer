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

    public async Task CleanupNonExistingSearchIdsAsync()
    {
	    string cleanupMetadataQuery = @"delete from sonicserver_indexed_search sis 
										where sis.type ='track' 
										and not exists(select 1 from metadata m
											           where m.metadataid = sis.id)";

	    string cleanupArtistsQuery = @"delete from sonicserver_indexed_search sis 
									   where sis.type ='artist' 
									   and not exists(select 1 from artists a
									   		          where a.artistid = sis.id)";
	    
	    string cleanupAlbumsQuery = @"delete from sonicserver_indexed_search sis 
									  where sis.type ='album' 
									  and not exists(select 1 from albums ab
									  		         where ab.albumid = sis.id)";
	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.ExecuteAsync(cleanupMetadataQuery);
	    await conn.ExecuteAsync(cleanupArtistsQuery);
	    await conn.ExecuteAsync(cleanupAlbumsQuery);
    }

    public async Task UpdateAlbumsYearAsync()
    {
	    string query = @"update albums al set (year) = (
						     select coalesce(max(tag_year), 0)
						     from metadata m
						     where m.AlbumId = al.AlbumId
						       and m.tag_year > 0
						 )
						 where al.year = 0";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.ExecuteAsync(query);
    }
    public async Task UpdateGappedRecordIdMetadataAsync()
    {
	    //what this essentially does is grabbing the highest record_id's
	    //moving them into deleted record_id's (gaps)
	    //some projects when they ask for 200 records, they except 200 records (kind of obvious)
	    //when there are gaps and say you have 200 albums it might show statistically only 49
	    //because record_id was deleted/cleaned up
	    //then second query to reset the record_id sequence ordering for new records
	    
	    string query = @"WITH gaps AS (
						   SELECT
						     record_id AS gap_id,
						     ROW_NUMBER() OVER (ORDER BY record_id) AS rn
						   FROM (
						     SELECT generate_series(MIN(record_id), MAX(record_id)) AS record_id
						     FROM metadata
						     EXCEPT
						     SELECT record_id FROM metadata
						   ) missing
						 ),
						 highs AS (
						   SELECT
						     record_id AS high_id,
						     ROW_NUMBER() OVER (ORDER BY record_id DESC) AS rn
						   FROM metadata
						   ORDER BY record_id DESC
						   LIMIT (SELECT COUNT(*) FROM gaps)
						 )
						 UPDATE metadata m
						 SET record_id = g.gap_id
						 FROM gaps g
						 JOIN highs h ON g.rn = h.rn
						 WHERE m.record_id = h.high_id";

	    string resetIdentityQuery = @"SELECT setval(
									      'metadata_record_id_seq',
									      (SELECT max(record_id) FROM metadata)
									  );";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.ExecuteAsync(query);
	    await conn.ExecuteAsync(resetIdentityQuery);
    }
    
    public async Task UpdateGappedRecordIdArtistsAsync()
    {
	    //what this essentially does is grabbing the highest record_id's
	    //moving them into deleted record_id's (gaps)
	    //some projects when they ask for 200 records, they except 200 records (kind of obvious)
	    //when there are gaps and say you have 200 albums it might show statistically only 49
	    //because record_id was deleted/cleaned up
	    //then second query to reset the record_id sequence ordering for new records
	    
	    string query = @"WITH gaps AS (
						   SELECT
						     record_id AS gap_id,
						     ROW_NUMBER() OVER (ORDER BY record_id) AS rn
						   FROM (
						     SELECT generate_series(MIN(record_id), MAX(record_id)) AS record_id
						     FROM artists
						     EXCEPT
						     SELECT record_id FROM artists
						   ) missing
						 ),
						 highs AS (
						   SELECT
						     record_id AS high_id,
						     ROW_NUMBER() OVER (ORDER BY record_id DESC) AS rn
						   FROM artists
						   ORDER BY record_id DESC
						   LIMIT (SELECT COUNT(*) FROM gaps)
						 )
						 UPDATE artists m
						 SET record_id = g.gap_id
						 FROM gaps g
						 JOIN highs h ON g.rn = h.rn
						 WHERE m.record_id = h.high_id";
	    
	    string resetIdentityQuery = @"SELECT setval(
									      'artists_record_id_seq',
									      (SELECT max(record_id) FROM artists)
									  );";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.ExecuteAsync(query);
	    await conn.ExecuteAsync(resetIdentityQuery);
    }
    
    public async Task UpdateGappedRecordIdAlbumsAsync()
    {
	    //what this essentially does is grabbing the highest record_id's
	    //moving them into deleted record_id's (gaps)
	    //some projects when they ask for 200 records, they except 200 records (kind of obvious)
	    //when there are gaps and say you have 200 albums it might show statistically only 49
	    //because record_id was deleted/cleaned up
	    //then second query to reset the record_id sequence ordering for new records
	    
	    string query = @"WITH gaps AS (
						   SELECT
						     record_id AS gap_id,
						     ROW_NUMBER() OVER (ORDER BY record_id) AS rn
						   FROM (
						     SELECT generate_series(MIN(record_id), MAX(record_id)) AS record_id
						     FROM albums
						     EXCEPT
						     SELECT record_id FROM albums
						   ) missing
						 ),
						 highs AS (
						   SELECT
						     record_id AS high_id,
						     ROW_NUMBER() OVER (ORDER BY record_id DESC) AS rn
						   FROM albums
						   ORDER BY record_id DESC
						   LIMIT (SELECT COUNT(*) FROM gaps)
						 )
						 UPDATE albums m
						 SET record_id = g.gap_id
						 FROM gaps g
						 JOIN highs h ON g.rn = h.rn
						 WHERE m.record_id = h.high_id";

	    string resetIdentityQuery = @"SELECT setval(
									      'albums_record_id_seq',
									      (SELECT max(record_id) FROM albums)
									  );";
	    
	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.ExecuteAsync(query);
	    await conn.ExecuteAsync(resetIdentityQuery);
    }
}