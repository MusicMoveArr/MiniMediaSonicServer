using MiniMediaSonicServer.Application.Attributes;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

[HybridBind]
public class GetUserRequest : SubsonicAuthModel
{
    public string Username { get; set; }
}