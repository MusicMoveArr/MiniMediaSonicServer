namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class GetSimilarSongsRequest
{
    public Guid Id { get; set; }
    public int Count { get; set; }
}