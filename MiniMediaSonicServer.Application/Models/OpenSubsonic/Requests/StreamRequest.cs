using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class StreamRequest : SubsonicAuthModel
{
    public Guid Id { get; set; }
    public int MaxBitRate { get; set; }
    public string Format { get; set; }
}