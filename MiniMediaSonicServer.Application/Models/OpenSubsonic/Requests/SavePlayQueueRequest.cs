using System.Text.Json.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class SavePlayQueueRequest
{
    public string Id { get; set; }
    public long Position { get; set; }
    public Guid? Current { get; set; }
}