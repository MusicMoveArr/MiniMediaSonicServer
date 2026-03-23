using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class DownloadController : SonicControllerBase
{
    private readonly StreamService _streamService;
    public DownloadController(StreamService streamService)
    {
        _streamService = streamService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] StreamRequest request)
    {
        var path = await _streamService.GetTrackPathByIdResponseAsync(request.Id);

        if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
        {
            return SubsonicResults.Fail(HttpContext, 0, "Track not found");
        }
        
        var contentType = ContentTypeFromSuffix(Path.GetExtension(path).TrimStart('.'));
        return Results.File(path, contentType, enableRangeProcessing: true);
    }
    
    private static string ContentTypeFromSuffix(string? suffix)
    {
        suffix = (suffix ?? "").Trim().TrimStart('.').ToLowerInvariant();
        return suffix switch
        {
            "mp3" => "audio/mpeg",
            "m4a" => "audio/mp4",
            "mp4" => "audio/mp4",
            "flac" => "audio/flac",
            "ogg" => "audio/ogg",
            "opus" => "audio/ogg",
            "wav" => "audio/wav",
            _ => "application/octet-stream"
        };
    }
}