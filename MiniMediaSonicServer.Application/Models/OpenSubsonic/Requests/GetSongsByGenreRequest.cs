using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class GetSongsByGenreRequest : SubsonicAuthModel
{
    public string Genre { get; set; }
    public int Count { get; set; } = 10;
    public int Offset { get; set; }
    public int MusicFolderId { get; set; }
}