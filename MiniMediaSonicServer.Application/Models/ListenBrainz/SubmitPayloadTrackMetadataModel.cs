using System.Text.Json.Serialization;

namespace MiniMediaSonicServer.Application.Models.ListenBrainz;

public class SubmitPayloadTrackMetadataModel
{
    [JsonPropertyName("additional_info")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required SubmitPayloadTrackMetadataAdditionalInfoModel AdditionalInfo { get; init; }
    
    [JsonPropertyName("artist_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string ArtistName { get; init; }
    
    [JsonPropertyName("track_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string TrackName { get; init; }
    
    [JsonPropertyName("release_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string ReleaseName { get; init; }
}