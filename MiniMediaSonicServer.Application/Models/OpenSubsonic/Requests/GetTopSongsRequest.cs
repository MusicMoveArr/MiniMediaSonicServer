using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class GetTopSongsRequest : SubsonicAuthModel
{
    public string Artist { get; set; }
    public int Count { get; set; }
}