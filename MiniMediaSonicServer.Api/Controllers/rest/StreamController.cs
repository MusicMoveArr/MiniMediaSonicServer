using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class StreamController : SonicControllerBase
{
    private readonly StreamService _streamService;
    private readonly TranscodeService _transcodeService;
    public StreamController(StreamService streamService,
        TranscodeService transcodeService)
    {
        _streamService = streamService;
        _transcodeService = transcodeService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] StreamRequest request)
    {
        var path = await _streamService.GetTrackPathByIdResponseAsync(request.Id);

        if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
        {
            return SubsonicResults.Fail(HttpContext, 0, "Track not found");
        }

        if (!string.IsNullOrWhiteSpace(request.Format) && !path.EndsWith(request.Format))
        {
            byte[]? bytes = await _transcodeService.TranscodeAsync(path, request.Format, request.MaxBitRate);
            if (bytes?.Length > 0)
            {
                if (request.Format == "aac")
                {
                    request.Format = "m4a";
                }
                var transcodedContentType = ContentTypeFromSuffix(request.Format);
                return Results.File(bytes, transcodedContentType, enableRangeProcessing: true);
            }
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