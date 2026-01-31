using System.Text.Json.Serialization;

namespace MiniMediaSonicServer.Application.Models.ListenBrainz;

public class SubmitPayloadModel
{
    [JsonPropertyName("listened_at")]
    public long ListenedAt { get; set; }
    
    [JsonPropertyName("track_metadata")]
    public SubmitPayloadTrackMetadataModel TrackMetadata { get; set; }
}