using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class User
{
    [JsonPropertyName("username")]
    [XmlElement("username")]
    public required string Username { get; init; }
    
    [JsonPropertyName("email")]
    [XmlElement("email")]
    public required string Email { get; init; }

    [JsonPropertyName("scrobblingEnabled")]
    [XmlElement("scrobblingEnabled")]
    public bool ScrobblingEnabled { get; set; }

    [JsonPropertyName("adminRole")]
    [XmlElement("adminRole")]
    public required bool AdminRole { get; init; }

    [JsonPropertyName("settingsRole")]
    [XmlElement("settingsRole")]
    public required bool SettingsRole { get; init; }

    [JsonPropertyName("downloadRole")]
    [XmlElement("downloadRole")]
    public required bool DownloadRole { get; init; }

    [JsonPropertyName("uploadRole")]
    [XmlElement("uploadRole")]
    public required bool UploadRole { get; init; }

    [JsonPropertyName("playlistRole")]
    [XmlElement("playlistRole")]
    public bool PlaylistRole { get; set; }

    [JsonPropertyName("coverArtRole")]
    [XmlElement("coverArtRole")]
    public required bool CoverArtRole { get; init; }

    [JsonPropertyName("commentRole")]
    [XmlElement("commentRole")]
    public required bool CommentRole { get; init; }

    [JsonPropertyName("podcastRole")]
    [XmlElement("podcastRole")]
    public required bool PodcastRole { get; init; }

    [JsonPropertyName("streamRole")]
    [XmlElement("streamRole")]
    public required bool StreamRole { get; init; }

    [JsonPropertyName("jukeboxRole")]
    [XmlElement("jukeboxRole")]
    public required bool JukeboxRole { get; init; }

    [JsonPropertyName("shareRole")]
    [XmlElement("shareRole")]
    public required bool ShareRole { get; init; }

    [JsonPropertyName("videoConversionRole")]
    [XmlElement("videoConversionRole")]
    public required bool VideoConversionRole { get; init; }

    [JsonPropertyName("folder")]
    [XmlElement("folder")]
    public List<int> Folder { get; init; } = [ 1 ];

    [JsonPropertyName("maxBitRate")]
    [XmlElement("maxBitRate")]
    public required int MaxBitRate { get; init; }
}