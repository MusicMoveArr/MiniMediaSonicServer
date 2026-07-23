using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class CreateShareRequest : SubsonicAuthModel
{
    public Guid Id { get; set; }
    public required string Description { get; set; }
    public long? Expires { get; set; }
}