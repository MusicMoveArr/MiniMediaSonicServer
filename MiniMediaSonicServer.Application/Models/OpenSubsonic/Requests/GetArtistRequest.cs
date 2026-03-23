using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class GetArtistRequest : SubsonicAuthModel
{
    public Guid Id { get; set; }
}