using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class NameIdEntity
{
    [XmlAttribute("id")]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [XmlAttribute("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    public NameIdEntity()
    {
        
    }

    public NameIdEntity(Guid id, string name)
    {
        this.Id = id;
        this.Name = name;
    }
}