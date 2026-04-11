using MiniMediaSonicServer.Application.Attributes;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Application.Models.Shares;

[HybridBind]
public class StreamShareRequest : SubsonicAuthModel
{
    public Guid Id { get; set; }
    public string ShareName { get; set; }
    public int MaxBitRate { get; set; }
    public string Format { get; set; }
}