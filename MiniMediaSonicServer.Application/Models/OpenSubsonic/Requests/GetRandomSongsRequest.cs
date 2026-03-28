using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class GetRandomSongsRequest : SubsonicAuthModel
{
    public int Size { get; set; }
    public string Genre { get; set; }
    public int FromYear { get; set; }
    public int ToYear { get; set; }
    public int MusicFolderId { get; set; }
}