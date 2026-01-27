using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Api.Controllers;

public abstract class SonicControllerBase : ControllerBase
{
    protected UserModel User => HttpContext.Items["user"] as UserModel;
    
    protected User GetUserModel()
    {
        return new User
        {
            Username = User.Username
        };
    }
}