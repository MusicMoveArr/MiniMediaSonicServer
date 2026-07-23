using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class DeleteUserRequest : SubsonicAuthModel
{
    public required string Username { get; set; }
}