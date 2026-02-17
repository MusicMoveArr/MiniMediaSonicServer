namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class GetTopSongsRequest
{
    public string Artist { get; set; }
    public int Count { get; set; }
}