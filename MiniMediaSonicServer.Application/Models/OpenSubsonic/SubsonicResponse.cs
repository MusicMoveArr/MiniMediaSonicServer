using System.Text.Json.Serialization;
using System.Xml.Serialization;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic;

[XmlRoot("subsonic-response")]
public class SubsonicResponse
{
    public SubsonicResponse()
    {
        Xmlns = "http://subsonic.org/restapi";
        Status = "ok";
        Version = "1.16.1";
        Type = "navidrome";
        ServerVersion = "0.51.0";
        OpenSubsonic = true;

        License = new License();
    }

    public SubsonicResponse(User user)
        : this()
    {
        this.User = user;
    }

    [XmlAttribute("xmlns")]
    [JsonIgnore]
    public string Xmlns { get; set; }

    [XmlAttribute("status")]
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [XmlAttribute("version")]
    [JsonPropertyName("version")]
    public string Version { get; set; }

    [XmlAttribute("type")]
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [XmlAttribute("serverVersion")]
    [JsonPropertyName("serverVersion")]
    public string? ServerVersion { get; set; }

    [XmlAttribute("openSubsonic")]
    [JsonPropertyName("openSubsonic")]
    public bool OpenSubsonic { get; set; }

    [XmlElement("error")]
    [JsonPropertyName("error")]
    public Error? Error { get; set; }

    [XmlElement("license")]
    [JsonPropertyName("license")]
    public License? License { get; set; }

    [XmlElement("openSubsonicExtensions")]
    [JsonPropertyName("openSubsonicExtensions")]
    public List<Extension>? OpenSubsonicExtensions { get; set; }

    [XmlElement("user")]
    [JsonPropertyName("user")]
    public User? User { get; set; }

    [XmlElement("albumList2")]
    [JsonPropertyName("albumList2")]
    public AlbumList2Response AlbumList2 { get; set; }

    [XmlElement("album")]
    [JsonPropertyName("album")]
    public AlbumID3? Album { get; set; }
    
    [XmlElement("musicFolders")]
    [JsonPropertyName("musicFolders")]
    public MusicFolders? MusicFolders { get; set; }
    
    [XmlElement("artist")]
    [JsonPropertyName("artist")]
    public ArtistID3? Artist { get; set; }

    [XmlElement("artists")]
    [JsonPropertyName("artists")]
    public ArtistsList? Artists { get; set; }

    [XmlElement("similarSongs2")]
    [JsonPropertyName("similarSongs2")]
    public SimilarSongsList2Response? SimilarSongsList2 { get; set; }
    
    [XmlElement("genres")]
    [JsonPropertyName("genres")]
    public Genres? Genres { get; set; }
}