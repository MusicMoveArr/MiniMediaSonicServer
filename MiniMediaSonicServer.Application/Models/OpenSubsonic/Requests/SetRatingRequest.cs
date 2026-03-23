using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class SetRatingRequest : SubsonicAuthModel
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
}