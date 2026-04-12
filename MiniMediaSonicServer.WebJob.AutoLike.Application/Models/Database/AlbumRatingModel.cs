namespace MiniMediaSonicServer.WebJob.AutoLike.Application.Models.Database;

public class AlbumRatingModel
{
    public Guid AlbumId { get; set; }
    public int AvgRating { get; set; }
}