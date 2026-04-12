using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class UpdateShareRequest : SubsonicAuthModel
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public long? Expires { get; set; }
}