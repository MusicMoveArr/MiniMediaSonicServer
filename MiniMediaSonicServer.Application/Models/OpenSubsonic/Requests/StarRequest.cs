namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class StarRequest
{
    public List<Guid> Id { get; set; }
    public List<Guid> AlbumId { get; set; }
    public List<Guid> ArtistId { get; set; }
}