using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class MusicFolder
{
    [XmlAttribute("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [XmlAttribute("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    public MusicFolder()
    {
        
    }

    public MusicFolder(int id, string name)
    {
        this.Id = id;
        this.Name = name;
    }
}