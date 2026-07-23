namespace MiniMediaSonicServer.Application.Models.Database;

public class UserModel
{
    public required Guid UserId { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required DateTime CreationDateTime { get; init; }
    public required string TokenBasedAuth { get; init; }
    public required string ListenBrainzUserToken { get; init; }
    
    public required bool AdminRole { get; init; }
    public required bool SettingsRole { get; init; }
    public required bool StreamRole { get; init; }
    public required bool JukeboxRole { get; init; }
    public required bool DownloadRole { get; init; }
    public required bool UploadRole { get; init; }
    public required bool CoverArtRole { get; init; }
    public required bool CommentRole { get; init; }
    public required bool PodcastRole { get; init; }
    public required bool ShareRole { get; init; }
    public required bool VideoConversionRole { get; init; }
    public required int MusicFolderId { get; init; }
    public required int MaxBitRate { get; init; }
    
    public required string MalojaUrl { get; init; }
    public required string MalojaApiKey { get; init; }
    
    //current request
    public required string ClientName { get; set; }
}