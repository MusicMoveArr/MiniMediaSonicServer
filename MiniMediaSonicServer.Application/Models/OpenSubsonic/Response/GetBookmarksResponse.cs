using System.Text.Json.Serialization;
using System.Xml.Serialization;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;

public class GetBookmarksResponse
{
    [XmlElement("bookmark")]
    [JsonPropertyName("bookmark")]
    public List<Bookmark> Bookmarks { get; set; }
}