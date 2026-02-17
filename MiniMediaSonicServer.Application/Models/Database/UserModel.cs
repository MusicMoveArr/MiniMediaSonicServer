namespace MiniMediaSonicServer.Application.Models.Database;

public class UserModel
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreationDateTime { get; set; }
    public string TokenBasedAuth { get; set; }
    public string ListenBrainzUserToken { get; set; }
    
    public bool AdminRole { get; set; }
    public bool SettingsRole { get; set; }
    public bool StreamRole { get; set; }
    public bool JukeboxRole { get; set; }
    public bool DownloadRole { get; set; }
    public bool UploadRole { get; set; }
    public bool CoverArtRole { get; set; }
    public bool CommentRole { get; set; }
    public bool PodcastRole { get; set; }
    public bool ShareRole { get; set; }
    public bool VideoConversionRole { get; set; }
    public int MusicFolderId { get; set; }
    public int MaxBitRate { get; set; }
    
    public string MalojaUrl { get; set; }
    public string MalojaApiKey { get; set; }
}