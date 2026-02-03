using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class User
{
    [JsonPropertyName("username")]
    [XmlElement("username")]
    public string Username { get; set; }
    
    [JsonPropertyName("email")]
    [XmlElement("email")]
    public string Email { get; set; }

    [JsonPropertyName("scrobblingEnabled")]
    [XmlElement("scrobblingEnabled")]
    public bool ScrobblingEnabled { get; set; }

    [JsonPropertyName("adminRole")]
    [XmlElement("adminRole")]
    public bool AdminRole { get; set; }

    [JsonPropertyName("settingsRole")]
    [XmlElement("settingsRole")]
    public bool SettingsRole { get; set; }

    [JsonPropertyName("downloadRole")]
    [XmlElement("downloadRole")]
    public bool DownloadRole { get; set; }

    [JsonPropertyName("uploadRole")]
    [XmlElement("uploadRole")]
    public bool UploadRole { get; set; }

    [JsonPropertyName("playlistRole")]
    [XmlElement("playlistRole")]
    public bool PlaylistRole { get; set; }

    [JsonPropertyName("coverArtRole")]
    [XmlElement("coverArtRole")]
    public bool CoverArtRole { get; set; }

    [JsonPropertyName("commentRole")]
    [XmlElement("commentRole")]
    public bool CommentRole { get; set; }

    [JsonPropertyName("podcastRole")]
    [XmlElement("podcastRole")]
    public bool PodcastRole { get; set; }

    [JsonPropertyName("streamRole")]
    [XmlElement("streamRole")]
    public bool StreamRole { get; set; }

    [JsonPropertyName("jukeboxRole")]
    [XmlElement("jukeboxRole")]
    public bool JukeboxRole { get; set; }

    [JsonPropertyName("shareRole")]
    [XmlElement("shareRole")]
    public bool ShareRole { get; set; }

    [JsonPropertyName("videoConversionRole")]
    [XmlElement("videoConversionRole")]
    public bool VideoConversionRole { get; set; }

    [JsonPropertyName("folder")]
    [XmlElement("folder")]
    public List<int> Folder { get; set; } = new() { 1 };

    [JsonPropertyName("maxBitRate")]
    [XmlElement("maxBitRate")]
    public int MaxBitRate { get; set; }
}