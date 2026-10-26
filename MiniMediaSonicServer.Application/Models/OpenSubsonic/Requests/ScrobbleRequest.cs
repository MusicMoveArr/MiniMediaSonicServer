using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class ScrobbleRequest : SubsonicAuthModel
{
    public List<Guid> Id { get; set; }
    public List<string> Time { get; set; }
    public bool Submission { get; set; } = true;
}