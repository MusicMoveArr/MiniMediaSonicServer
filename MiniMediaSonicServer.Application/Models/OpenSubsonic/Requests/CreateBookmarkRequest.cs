using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class CreateBookmarkRequest : SubsonicAuthModel
{
    public required Guid Id { get; set; }
    public required long Position { get; set; }
    public required string Comment { get; set; }
}