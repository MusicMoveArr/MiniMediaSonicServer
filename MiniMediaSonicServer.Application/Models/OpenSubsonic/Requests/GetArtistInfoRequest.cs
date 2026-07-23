using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class GetArtistInfoRequest : SubsonicAuthModel
{
    public Guid Id { get; set; }
    public int Count { get; set; }
    public required string IncludeNotPresent { get; set; }
}