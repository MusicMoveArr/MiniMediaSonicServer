namespace MiniMediaSonicServer.Application.Models.Database;

public class UserModel
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreationDateTime { get; set; }
}