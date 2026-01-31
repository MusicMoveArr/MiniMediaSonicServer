using System.Text.Json.Serialization;

namespace MiniMediaSonicServer.Application.Models.ListenBrainz;

public class SubmitModel
{
    [JsonPropertyName("listen_type")]
    public string ListenType { get; set; }
    
    [JsonPropertyName("payload")]
    public List<SubmitPayloadModel> Payload { get; set; }
}