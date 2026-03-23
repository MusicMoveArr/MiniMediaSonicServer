using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class GetSimilarSongs2Request : SubsonicAuthModel
{
    public Guid Id { get; set; }
    public int Count { get; set; }
}