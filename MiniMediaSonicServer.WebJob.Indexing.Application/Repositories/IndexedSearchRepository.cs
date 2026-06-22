using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using Npgsql;

namespace MiniMediaSonicServer.WebJob.Indexing.Application.Repositories;

public class IndexedSearchRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    private readonly int MaxQueryTimeout = (int)TimeSpan.FromMinutes(30).TotalSeconds;
    
    public IndexedSearchRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
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
	    await conn.ExecuteAsync(query, commandTimeout: MaxQueryTimeout);
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
	    await conn.ExecuteAsync(query, commandTimeout: MaxQueryTimeout);
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
        await conn.ExecuteAsync(query, commandTimeout: MaxQueryTimeout);
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
	    await conn.ExecuteAsync(query, commandTimeout: MaxQueryTimeout);
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
	    await conn.ExecuteAsync(query, commandTimeout: MaxQueryTimeout);
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
	    await conn.ExecuteAsync(query, commandTimeout: MaxQueryTimeout);
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
	    await conn.ExecuteAsync(cleanupMetadataQuery, commandTimeout: MaxQueryTimeout);
	    await conn.ExecuteAsync(cleanupArtistsQuery, commandTimeout: MaxQueryTimeout);
	    await conn.ExecuteAsync(cleanupAlbumsQuery, commandTimeout: MaxQueryTimeout);
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
	    await conn.ExecuteAsync(query, commandTimeout: MaxQueryTimeout);
    }
    public async Task UpdateGappedRecordIdMetadataAsync()
    {
	    //what this essentially does is grabbing the highest record_id's
	    //moving them into deleted record_id's (gaps)
	    //some projects when they ask for 200 records, they except 200 records (kind of obvious)
	    //when there are gaps and say you have 200 albums it might show statistically only 49
	    //because record_id was deleted/cleaned up
	    //then second query to reset the record_id sequence ordering for new records

	    string outOfSyncCountQuery = @"WITH gaps AS (
									  SELECT
									    record_id AS gap_id,
									    ROW_NUMBER() OVER (ORDER BY record_id) AS rn
									  FROM (
									    SELECT generate_series(1, MAX(record_id)) AS record_id
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
									SELECT count(*)
									FROM gaps g
									JOIN highs h ON g.rn = h.rn";
	    
	    
	    string resetRecordSeqId = @"SELECT setval('metadata_record_id_seq', 1, false)";
	    string resetRecordId = @"UPDATE metadata
								 SET record_id = sub.new_id
								 FROM (
								     SELECT record_id, row_number() OVER (ORDER BY record_id) + 1000000000 AS new_id
								     FROM metadata
								 ) sub
								 WHERE metadata.record_id = sub.record_id";
	    
	    string setNewRecordId = @"UPDATE metadata
								  SET record_id = nextval('metadata_record_id_seq')";
	    
	    string query = @"WITH gaps AS (
						   SELECT
						     record_id AS gap_id,
						     ROW_NUMBER() OVER (ORDER BY record_id) AS rn
						   FROM (
						     SELECT generate_series(1, MAX(record_id)) AS record_id
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
	    await conn.OpenAsync();
	    var transaction = await conn.BeginTransactionAsync();

	    try
	    {
		    int outOfSync = await conn.ExecuteScalarAsync<int>(outOfSyncCountQuery,
			    transaction: transaction);
		    if (outOfSync >= 100000)
		    {
			    //just too many records are out of sync, updating this
			    //amount takes too long to update everything, lazy update works faster
		    
			    await conn.ExecuteAsync(resetRecordSeqId,
				    transaction: transaction);
		    
			    await conn.ExecuteAsync(resetRecordId, 
				    commandTimeout: MaxQueryTimeout,
				    transaction: transaction);
		    
			    await conn.ExecuteAsync(setNewRecordId, 
				    commandTimeout: MaxQueryTimeout,
				    transaction: transaction);
		    }
		    else
		    {
			    await conn.ExecuteAsync(query, commandTimeout: MaxQueryTimeout,
				    transaction: transaction);
		    }
		    await conn.ExecuteAsync(resetIdentityQuery,
			    transaction: transaction);
		    await transaction.CommitAsync();
	    }
	    catch (Exception e)
	    {
		    await transaction.RollbackAsync();
		    Console.WriteLine(e.Message + "\r\n" + e.StackTrace);
	    }
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
						     SELECT generate_series(1, MAX(record_id)) AS record_id
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
	    await conn.ExecuteAsync(query, commandTimeout: MaxQueryTimeout);
	    await conn.ExecuteAsync(resetIdentityQuery, commandTimeout: MaxQueryTimeout);
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
						     SELECT generate_series(1, MAX(record_id)) AS record_id
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
	    await conn.ExecuteAsync(query, commandTimeout: MaxQueryTimeout);
	    await conn.ExecuteAsync(resetIdentityQuery);
    }
    
    public async Task UpdateRecordTitleAscIdAlbumsAsync()
    {
	    //update record_title_asc_id only if needed when the table is out of sync with title ordering asc
	    string outOfSyncQuery = @"WITH expected AS (
							SELECT
								albumid,
								record_title_asc_id,
								ROW_NUMBER() OVER (ORDER BY title ASC)          AS rank_by_title,
								ROW_NUMBER() OVER (ORDER BY record_title_asc_id ASC) AS rank_by_id
							FROM albums
							WHERE record_title_asc_id IS NOT NULL
						)
						SELECT COUNT(*) AS out_of_sync
						FROM expected
						WHERE rank_by_title != rank_by_id";

	    string resetTitleRecordAscSeqId = @"SELECT setval('albums_record_title_asc_id_seq', 1, false)";
	    string resetTitleRecordAscId = @"UPDATE albums
										 SET record_title_asc_id = sub.new_id
										 FROM (
										     SELECT record_title_asc_id, row_number() OVER (ORDER BY record_title_asc_id) + 1000000000 AS new_id
										     FROM albums
										 ) sub
										 WHERE albums.record_title_asc_id = sub.record_title_asc_id";
	    
	    string setNewTitleRecordAscId = @"WITH ordered AS (
										      SELECT
										          albumid,
										          nextval('albums_record_title_asc_id_seq') AS new_id
										      FROM albums
										      ORDER BY title ASC
										  )
										  UPDATE albums
										  SET record_title_asc_id = ordered.new_id
										  FROM ordered
										  WHERE albums.albumid = ordered.albumid";
	    
	    string resetIdentityQuery = @"SELECT setval(
									      'albums_record_title_asc_id_seq',
									      (SELECT max(record_title_asc_id) FROM albums)
									  );";
	    
	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    int outOfSync = await conn.ExecuteScalarAsync<int>(outOfSyncQuery);
	    if (outOfSync > 0)
	    {
		    await conn.ExecuteAsync(resetTitleRecordAscSeqId, commandTimeout: MaxQueryTimeout);
		    await conn.ExecuteAsync(resetTitleRecordAscId, commandTimeout: MaxQueryTimeout);
		    await conn.ExecuteAsync(setNewTitleRecordAscId, commandTimeout: MaxQueryTimeout);
		    await conn.ExecuteAsync(resetIdentityQuery, commandTimeout: MaxQueryTimeout);
	    }
    }
}