namespace MiniMediaSonicServer.Application.Models.Database;

public class ShareModel
{
    public Guid ShareId { get; set; }
    public Guid UserId { get; set; }
    public string ShareName { get; set; }
    public string Description { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastVisitedAt { get; set; }
    public int VisitCount { get; set; }
    public string Type { get; set; }
    public Guid MediaId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public string Username { get; set; }
}