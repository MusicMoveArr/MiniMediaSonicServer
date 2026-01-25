using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class User
{
    [JsonPropertyName("username")]
    [XmlElement("username")]
    public string Username { get; set; } = "admin";

    [JsonPropertyName("scrobblingEnabled")]
    [XmlElement("scrobblingEnabled")]
    public bool ScrobblingEnabled { get; set; } = true;

    [JsonPropertyName("adminRole")]
    [XmlElement("adminRole")]
    public bool AdminRole { get; set; } = true;

    [JsonPropertyName("settingsRole")]
    [XmlElement("settingsRole")]
    public bool SettingsRole { get; set; } = true;

    [JsonPropertyName("downloadRole")]
    [XmlElement("downloadRole")]
    public bool DownloadRole { get; set; } = true;

    [JsonPropertyName("uploadRole")]
    [XmlElement("uploadRole")]
    public bool UploadRole { get; set; } = true;

    [JsonPropertyName("playlistRole")]
    [XmlElement("playlistRole")]
    public bool PlaylistRole { get; set; } = true;

    [JsonPropertyName("coverArtRole")]
    [XmlElement("coverArtRole")]
    public bool CoverArtRole { get; set; } = true;

    [JsonPropertyName("commentRole")]
    [XmlElement("commentRole")]
    public bool CommentRole { get; set; } = true;

    [JsonPropertyName("podcastRole")]
    [XmlElement("podcastRole")]
    public bool PodcastRole { get; set; } = true;

    [JsonPropertyName("streamRole")]
    [XmlElement("streamRole")]
    public bool StreamRole { get; set; } = true;

    [JsonPropertyName("jukeboxRole")]
    [XmlElement("jukeboxRole")]
    public bool JukeboxRole { get; set; } = false;

    [JsonPropertyName("shareRole")]
    [XmlElement("shareRole")]
    public bool ShareRole { get; set; } = true;

    [JsonPropertyName("videoConversionRole")]
    [XmlElement("videoConversionRole")]
    public bool VideoConversionRole { get; set; } = true;

    [JsonPropertyName("folder")]
    [XmlElement("folder")]
    public List<int> Folder { get; set; } = new() { 1 };
}