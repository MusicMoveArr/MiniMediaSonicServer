using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class GetSongRequest : SubsonicAuthModel
{
    public Guid Id { get; set; }
}