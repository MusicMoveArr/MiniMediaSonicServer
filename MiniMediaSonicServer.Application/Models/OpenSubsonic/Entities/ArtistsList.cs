using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class ArtistsList
{
    [XmlAttribute("ignoredArticles")]
    [JsonPropertyName("ignoredArticles")]
    public string IgnoredArticles { get; set; } = string.Empty;

    [XmlElement("index")]
    [JsonPropertyName("index")]
    public List<Index> Index { get; set; } = new();
}