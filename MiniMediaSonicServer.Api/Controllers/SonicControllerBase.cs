using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Api.Controllers;

public abstract class SonicControllerBase : ControllerBase
{
    protected User GetUserModel()
    {
        var dbuser = HttpContext.Items["user"] as UserModel;

        return new User
        {
            Username = dbuser.Username
        };
    }
}