namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class StreamRequest
{
    public Guid Id { get; set; }
    public int MaxBitRate { get; set; }
    public string Format { get; set; }
}