using System.Text.Json.Serialization;
using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class SavePlayQueueByIndexRequest : SubsonicAuthModel
{
    public List<string>? Id { get; set; }
    public long? Position { get; set; }
    public int? CurrentIndex { get; set; }
}