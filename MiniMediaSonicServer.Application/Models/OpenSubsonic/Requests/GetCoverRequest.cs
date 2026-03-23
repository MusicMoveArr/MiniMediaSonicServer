using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class GetCoverRequest : SubsonicAuthModel
{
    public string Id { get; set; }
}