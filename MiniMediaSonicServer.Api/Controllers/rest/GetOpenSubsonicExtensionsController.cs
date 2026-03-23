using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[AllowAnonymous]
[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetOpenSubsonicExtensionsController : SonicControllerBase
{
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] SubsonicAuthModel request)
    {
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            OpenSubsonicExtensions = new List<Extension>
            {
                new Extension
                {
                    Name = "formPost",
                    Versions = [1]
                }
            }
        });
    }
}