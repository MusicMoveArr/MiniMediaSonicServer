using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using Microsoft.AspNetCore.Mvc.Filters;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Filters;

public sealed class SubsonicAuthFilter : IAsyncActionFilter
{
    private readonly UserService _userService;
    public SubsonicAuthFilter(UserService userService)
    {
        _userService = userService;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        bool hasAllowAnonymous = endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>() != null;

        if (hasAllowAnonymous)
        {
            await next();
            return;
        }
        
        var ctx = context.HttpContext;
        var q = ctx.Request.Query;
        var username = q["u"].FirstOrDefault() ?? "";
        var password = q["p"].FirstOrDefault() ?? "";
        var token = q["t"].FirstOrDefault() ?? "";
        var salt = q["s"].FirstOrDefault() ?? "";

        if (string.IsNullOrWhiteSpace(username))
        {
            context.Result = SubsonicResults.FailActionResult(ctx, 10, "Required parameter is missing");
            return;
        }
        if (!Regex.IsMatch(username, "^[a-zA-Z0-9_-]+$", RegexOptions.None, TimeSpan.FromMilliseconds(100)))
        {
            context.Result = SubsonicResults.FailActionResult(ctx, 40, "Wrong username or password");
            return;
        }

        var authenticated = false;
        var user = await _userService.GetUserByUsernameAsync(username);
        
        if (user == null)
        {
            context.Result = SubsonicResults.FailActionResult(ctx, 40, "Wrong username or password");
            return;
        }
        
        if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(salt))
        {
            authenticated = _userService.ValidateToken(user.TokenBasedAuth, token, salt);
        }
        else if (!string.IsNullOrWhiteSpace(password))
        {
            authenticated = _userService.ValidatePassword(password, user.Password);
        }

        if (!authenticated)
        {
            context.Result = SubsonicResults.FailActionResult(ctx, 40, "Wrong username or password");
            return;
        }

        ctx.Items["user"] = user;
        await next();
    }
}