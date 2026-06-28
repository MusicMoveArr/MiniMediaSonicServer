using Dapper;
using DapperBulkQueries.Common;
using DapperBulkQueries.Npgsql;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Helpers;
using MiniMediaSonicServer.WebJob.Indexing.Application.Models;
using Npgsql;

namespace MiniMediaSonicServer.WebJob.Indexing.Application.Repositories;

public class IndexedTrackSonicRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    
    public IndexedTrackSonicRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }

    public async Task<List<Guid>> GetTrackIdsToIndexAsync()
    {
	    string query = @"select MetadataId
						 from metadata m 
						 where not exists (
						 		select true from sonicserver_indexed_track_sonic
						 			where TrackId = m.MetadataId)
						 	and  exists (
						 		select true from metadata_mood
						 			where MetadataId = m.MetadataId)
						 limit 1000";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    return (await conn.QueryAsync<Guid>(query)).ToList();
    }

    public async Task<List<SonicTrackModel>> GetRelatedTracksAsync(Guid trackId)
    {
	    string query = @"WITH source AS (
						    SELECT mood.*, m.title
						    FROM metadata_mood mood
							join metadata m on m.metadataid = mood.metadataid
							WHERE mood.MetadataId = @trackId
						)
						select 
							t.MetadataId as TrackId,
						    m.title,
						      ABS((t.mood_happy->>'happy')::float - (s.mood_happy->>'happy')::float)
						    + ABS((t.mood_happy->>'non_happy')::float - (s.mood_happy->>'non_happy')::float)

						    + ABS((t.mood_sad->>'sad')::float - (s.mood_sad->>'sad')::float)
						    + ABS((t.mood_sad->>'non_sad')::float - (s.mood_sad->>'non_sad')::float)

						    + ABS((t.mood_aggressive->>'aggressive')::float - (s.mood_aggressive->>'aggressive')::float)
						    + ABS((t.mood_aggressive->>'not_aggressive')::float - (s.mood_aggressive->>'not_aggressive')::float)

						    + ABS((t.mood_relaxed->>'relaxed')::float - (s.mood_relaxed->>'relaxed')::float)
						    + ABS((t.mood_relaxed->>'non_relaxed')::float - (s.mood_relaxed->>'non_relaxed')::float)

						    + ABS((t.mood_acoustic->>'acoustic')::float - (s.mood_acoustic->>'acoustic')::float)
						    + ABS((t.mood_acoustic->>'non_acoustic')::float - (s.mood_acoustic->>'non_acoustic')::float)

						    + ABS((t.mood_electronic->>'electronic')::float - (s.mood_electronic->>'electronic')::float)
						    + ABS((t.mood_electronic->>'non_electronic')::float - (s.mood_electronic->>'non_electronic')::float)

						    + ABS((t.mood_party->>'party')::float - (s.mood_party->>'party')::float)
						    + ABS((t.mood_party->>'non_party')::float - (s.mood_party->>'non_party')::float)

						    + ABS((t.ability_approach->>'approachable')::float - (s.ability_approach->>'approachable')::float)
						    + ABS((t.ability_approach->>'moderately approachable')::float - (s.ability_approach->>'moderately approachable')::float)
						    + ABS((t.ability_approach->>'not approachable')::float - (s.ability_approach->>'not approachable')::float)

						    + ABS((t.ability_dance->>'danceable')::float - (s.ability_dance->>'danceable')::float)
						    + ABS((t.ability_dance->>'not_danceable')::float - (s.ability_dance->>'not_danceable')::float)

						    + ABS((t.voice_instrumental->>'voice')::float - (s.voice_instrumental->>'voice')::float)
						    + ABS((t.voice_instrumental->>'instrumental')::float - (s.voice_instrumental->>'instrumental')::float)

						    + ABS((t.timbre->>'bright')::float - (s.timbre->>'bright')::float)
						    + ABS((t.timbre->>'dark')::float - (s.timbre->>'dark')::float)

						    + ABS((t.engagement_3c->>'engaging')::float - (s.engagement_3c->>'engaging')::float)
						    + ABS((t.engagement_3c->>'moderately engaging')::float - (s.engagement_3c->>'moderately engaging')::float)
						    + ABS((t.engagement_3c->>'not engaging')::float - (s.engagement_3c->>'not engaging')::float)

						    + ABS((t.engagement_regression->>'engagement')::float - (s.engagement_regression->>'engagement')::float)

						    + ABS((t.gender->>'male')::float - (s.gender->>'male')::float)
						    + ABS((t.gender->>'female')::float - (s.gender->>'female')::float)
								AS distance
						FROM metadata_mood t
						CROSS JOIN source s
						join metadata s2 on s2.metadataid = s.metadataid 
						join metadata m on m.metadataid = t.metadataid 
						WHERE t.MetadataId <> s.MetadataId
							and similarity(m.title, s.title) < 0.90 --preventing first record being the source track
						ORDER BY distance ASC
						LIMIT 100";

	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    var dbTracks = 
		    (await conn.QueryAsync<SonicTrackModel>(query, param: new { trackId }))
		    .ToList();

	    var deduplicatedTracks = new List<SonicTrackModel>();

	    foreach (var track in dbTracks.Where(t => t.Distance < 1.0f))
	    {
		    bool exists = deduplicatedTracks
			    .Any(t => FuzzyHelper.FuzzRatioToLower(t.Title, track.Title) > 95);
		    
		    if (exists)
		    {
			    continue;
		    }
		    deduplicatedTracks.Add(track);
	    }

	    return deduplicatedTracks;
    }
    
    public async Task BulkInsertTracksAsync(List<IndexedTrackSonicModel> tracks)
    {
	    await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
	    await conn.ExecuteBulkInsertAsync(
		    "sonicserver_indexed_track_sonic",
		    tracks,
		    ["TrackId", "RelatedTrackId", "Distance"], 
		    onConflict: OnConflict.DoNothing);
    }
}