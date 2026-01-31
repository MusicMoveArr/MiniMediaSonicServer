using System.Text.Json.Serialization;

namespace MiniMediaSonicServer.Application.Models.ListenBrainz;

public class SubmitPayloadTrackMetadataModel
{
    [JsonPropertyName("additional_info")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SubmitPayloadTrackMetadataAdditionalInfoModel AdditionalInfo { get; set; }
    
    [JsonPropertyName("artist_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ArtistName { get; set; }
    
    [JsonPropertyName("track_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string TrackName { get; set; }
    
    [JsonPropertyName("release_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ReleaseName { get; set; }
}