using MiniMediaSonicServer.Application.Attributes;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Application.Models.Handlers.LibreFm;

[HybridBind]
public class ConnectionLibreFmRequest : SubsonicAuthModel
{
    public required string LibreFmApiKey { get; set; }
    public required string LibreFmApiSecret { get; set; }
}