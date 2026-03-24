using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[AllowAnonymous]
[ApiController]
[Route("/")]
public class DefaultController : SonicControllerBase
{
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] SubsonicAuthModel request)
    {
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}