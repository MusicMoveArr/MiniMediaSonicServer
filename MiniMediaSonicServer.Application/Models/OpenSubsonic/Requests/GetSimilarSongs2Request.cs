namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class GetSimilarSongs2Request
{
    public Guid Id { get; set; }
    public int Count { get; set; }
}