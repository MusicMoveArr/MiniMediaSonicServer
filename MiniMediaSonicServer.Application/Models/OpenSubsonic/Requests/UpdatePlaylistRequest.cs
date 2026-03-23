using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class UpdatePlaylistRequest : SubsonicAuthModel
{
    public Guid PlaylistId { get; set; }
    public string? Name { get; set; }
    public string? Comment { get; set; }
    public bool? Public { get; set; }
    public List<Guid>? SongIdToAdd { get; set; }
    public List<Guid>? SongIndexToRemove { get; set; }
}