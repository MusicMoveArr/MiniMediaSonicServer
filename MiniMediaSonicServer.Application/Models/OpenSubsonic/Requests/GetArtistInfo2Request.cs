using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class GetArtistInfo2Request : SubsonicAuthModel
{
    public Guid Id { get; set; }
    public int Count { get; set; }
    public bool IncludeNotPresent { get; set; }
}