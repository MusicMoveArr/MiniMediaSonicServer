using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class CreateShareController : SonicControllerBase
{
    [HttpGet, HttpPost]
    public async Task<IResult> Get()
    {
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse(GetUserModel()));
    }
}