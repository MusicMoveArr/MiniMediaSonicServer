using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class StarRequest : SubsonicAuthModel
{
    public List<Guid> Id { get; set; }
    public List<Guid> AlbumId { get; set; }
    public List<Guid> ArtistId { get; set; }
}