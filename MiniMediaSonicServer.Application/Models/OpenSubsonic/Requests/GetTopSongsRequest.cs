using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class GetTopSongsRequest : SubsonicAuthModel
{
    public required string Artist { get; set; }
    public required int Count { get; set; }
}