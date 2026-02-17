namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class CreateUserRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public bool? AdminRole { get; set; }
    public bool? SettingsRole { get; set; }
    public bool? StreamRole { get; set; }
    public bool? JukeboxRole { get; set; }
    public bool? DownloadRole { get; set; }
    public bool? UploadRole { get; set; }
    public bool? CoverArtRole { get; set; }
    public bool? CommentRole { get; set; }
    public bool? PodcastRole { get; set; }
    public bool? ShareRole { get; set; }
    public bool? VideoConversionRole { get; set; }
    public int? MusicFolderId { get; set; }
    public int? MaxBitRate { get; set; }
}