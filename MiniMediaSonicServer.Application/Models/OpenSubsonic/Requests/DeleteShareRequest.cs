using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class DeleteShareRequest : SubsonicAuthModel
{
    public Guid Id { get; set; }
}