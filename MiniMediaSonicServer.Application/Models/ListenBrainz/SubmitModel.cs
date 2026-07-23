using System.Text.Json.Serialization;

namespace MiniMediaSonicServer.Application.Models.ListenBrainz;

public class SubmitModel
{
    [JsonPropertyName("listen_type")]
    public required string ListenType { get; init; }
    
    [JsonPropertyName("payload")]
    public required List<SubmitPayloadModel> Payload { get; init; }
}