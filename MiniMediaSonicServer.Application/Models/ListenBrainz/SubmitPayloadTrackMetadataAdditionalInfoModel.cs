using System.Text.Json.Serialization;

namespace MiniMediaSonicServer.Application.Models.ListenBrainz;

public class SubmitPayloadTrackMetadataAdditionalInfoModel
{
    [JsonPropertyName("media_player")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string MediaPlayer { get; set; }
    
    [JsonPropertyName("submission_client")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string SubmissionClient { get; set; }
    
    [JsonPropertyName("submission_client_version")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string SubmissionClientVersion { get; set; }
    
    [JsonPropertyName("release_mbid")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ReleaseMbId { get; set; }
    
    [JsonPropertyName("artist_mbids")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? ArtistMbId { get; set; }
    
    [JsonPropertyName("recording_mbid")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string RecordingMbId { get; set; }
    
    [JsonPropertyName("tags")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> Tags { get; set; }
    
    [JsonPropertyName("duration_ms")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? DurationMs { get; set; }
}