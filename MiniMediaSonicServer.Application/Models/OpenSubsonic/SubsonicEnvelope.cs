using System.Text.Json.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic;

public sealed class SubsonicEnvelope
{
    [JsonPropertyName("subsonic-response")]
    public SubsonicResponse Response { get; set; } = new();
}