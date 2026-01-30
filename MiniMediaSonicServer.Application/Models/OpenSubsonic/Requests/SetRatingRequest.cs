namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

public class SetRatingRequest
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
}