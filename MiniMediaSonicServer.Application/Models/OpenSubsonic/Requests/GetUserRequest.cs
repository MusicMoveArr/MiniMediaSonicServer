using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class GetUserRequest : SubsonicAuthModel
{
    public required string Username { get; set; }
}