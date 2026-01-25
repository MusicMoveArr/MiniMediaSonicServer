using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using Microsoft.AspNetCore.Mvc.Filters;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Api.Filters;

public sealed class SubsonicAuthFilter : IAsyncActionFilter
{
    private readonly UserRepository _userRepository;
    public SubsonicAuthFilter(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var ctx = context.HttpContext;
        var q = ctx.Request.Query;
        var username = q["u"].FirstOrDefault() ?? "";
        var password = q["p"].FirstOrDefault() ?? "";
        var token = q["t"].FirstOrDefault() ?? "";
        var salt = q["s"].FirstOrDefault() ?? "";
        
        Console.WriteLine(ctx.Request.Path);

        if (string.IsNullOrWhiteSpace(username))
        {
            context.Result = new ObjectResult(SubsonicResults.Fail(ctx, 10, "Required parameter is missing"));
            return;
        }

        var authenticated = false;
        var user = await _userRepository.GetUserByUsernameAsync(username);

        if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(salt))
        {
            authenticated = ValidateToken(username, token, salt);
        }
        else if (!string.IsNullOrWhiteSpace(password))
        {
            authenticated = ValidatePassword(password, user.Password);
        }

        if (!authenticated)
        {
            context.Result = new ObjectResult(SubsonicResults.Fail(ctx, 40, "Wrong username or password"));
            return;
        }

        ctx.Items["user"] = user;
        await next();
    }

    private bool ValidateToken(string username, string token, string salt)
    {
        //not implemented yet
        return true;
    }

    private bool ValidatePassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}