using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class Genres
{
    [XmlElement("genre")]
    [JsonPropertyName("genre")]
    public List<Genre> Genre { get; set; } = new();
}