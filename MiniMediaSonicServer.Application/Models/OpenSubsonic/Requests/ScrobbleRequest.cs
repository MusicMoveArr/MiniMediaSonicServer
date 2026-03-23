using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class ScrobbleRequest : SubsonicAuthModel
{
    public Guid Id { get; set; }
    public long Time { get; set; }
    public bool Submission { get; set; }
}