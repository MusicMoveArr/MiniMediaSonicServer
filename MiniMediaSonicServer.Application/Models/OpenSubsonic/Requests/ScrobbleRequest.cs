namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class ScrobbleRequest
{
    public Guid Id { get; set; }
    public long Time { get; set; }
    public bool Submission { get; set; }
}