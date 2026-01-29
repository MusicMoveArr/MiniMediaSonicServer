using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class ArtistInfo2
{
    [XmlElement("biography")]
    [JsonPropertyName("biography")]
    public string? Biography { get; set; }

    [XmlElement("musicBrainzId")]
    [JsonPropertyName("musicBrainzId")]
    public string? MusicBrainzId { get; set; }

    [XmlElement("lastFmUrl")]
    [JsonPropertyName("lastFmUrl")]
    public string? LastFmUrl { get; set; }

    [XmlElement("smallImageUrl")]
    [JsonPropertyName("smallImageUrl")]
    public string? SmallImageUrl { get; set; }

    [XmlElement("mediumImageUrl")]
    [JsonPropertyName("mediumImageUrl")]
    public string? MediumImageUrl { get; set; }

    [XmlElement("largeImageUrl")]
    [JsonPropertyName("largeImageUrl")]
    public string? LargeImageUrl { get; set; }

    [XmlElement("similarArtist")]
    [JsonPropertyName("similarArtist")]
    public List<ArtistID3> SimilarArtist { get; set; } = new();
}