using System.Text.Json.Serialization;
using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class SavePlayQueueRequest : SubsonicAuthModel
{
    public List<string> Id { get; set; }
    public long Position { get; set; }
    public Guid? Current { get; set; }
}