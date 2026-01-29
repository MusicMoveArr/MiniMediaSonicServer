namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class GetArtistInfo2Request
{
    public Guid Id { get; set; }
    public int Count { get; set; }
    public bool IncludeNotPresent { get; set; }
}