using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class CreateBookmarkRequest : SubsonicAuthModel
{
    public Guid Id { get; set; }
    public long Position { get; set; }
    public string Comment { get; set; }
}