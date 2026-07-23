namespace MiniMediaSonicServer.Application.Models.Database;

public class ShareModel
{
    public required Guid ShareId { get; init; }
    public required Guid UserId { get; init; }
    public required string ShareName { get; init; }
    public required string Description { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public DateTime? LastVisitedAt { get; init; }
    public required int VisitCount { get; init; }
    public required string Type { get; init; }
    public required Guid MediaId { get; init; }
    public required bool IsDeleted { get; init; }
    public required DateTime? DeletedAt { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    
    public required string Username { get; init; }
}