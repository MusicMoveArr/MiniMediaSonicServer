using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class CreatePlaylistRequest : SubsonicAuthModel
{
    public Guid? PlaylistId { get; set; }
    public string Name { get; set; }
    public Guid SongId { get; set; }
}