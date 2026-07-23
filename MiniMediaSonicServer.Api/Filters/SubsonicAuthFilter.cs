using Microsoft.AspNetCore.Authorization;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using Microsoft.AspNetCore.Mvc.Filters;
using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Filters;

public sealed class SubsonicAuthFilter : IAsyncActionFilter
{
    private readonly UserService _userService;
    private const string GenericErrorResponseMessage = "Wrong username or password";
    
    public SubsonicAuthFilter(UserService userService)
    {
        _userService = userService;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var authModel = context.ActionArguments.Values.FirstOrDefault() as SubsonicAuthModel;
        var endpoint = context.HttpContext.GetEndpoint();
        var ctx = context.HttpContext;
        
        if (endpoint == null)
        {
            context.Result = SubsonicResults.FailActionResult(ctx, SubsonicErrorCode.WrongUsernameOrPassword, GenericErrorResponseMessage);
            return;
        }
        
        bool hasAllowAnonymous = endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>() != null;
        
        if (hasAllowAnonymous)
        {
            ctx.Items["format"] = authModel?.AuthOutputFormat;
            await next();
            return;
        }

        if (string.IsNullOrWhiteSpace(authModel.AuthUsername))
        {
            context.Result = SubsonicResults.FailActionResult(ctx, SubsonicErrorCode.WrongUsernameOrPassword, GenericErrorResponseMessage);
            return;
        }

        var authenticated = false;
        var user = await _userService.GetUserByUsernameAsync(authModel.AuthUsername);
        
        if (user == null)
        {
            context.Result = SubsonicResults.FailActionResult(ctx, SubsonicErrorCode.WrongUsernameOrPassword, GenericErrorResponseMessage);
            return;
        }
        
        if (!string.IsNullOrWhiteSpace(authModel.AuthToken) && !string.IsNullOrWhiteSpace(authModel.AuthSalt))
        {
            authenticated = _userService.ValidateToken(user.TokenBasedAuth, authModel.AuthToken, authModel.AuthSalt);
        }
        else if (!string.IsNullOrWhiteSpace(authModel.AuthPassword))
        {
            authenticated = _userService.ValidatePassword(authModel.AuthPassword, user.Password);
        }

        if (!authenticated)
        {
            context.Result = SubsonicResults.FailActionResult(ctx, SubsonicErrorCode.WrongUsernameOrPassword, GenericErrorResponseMessage);
            return;
        }

        user.ClientName = authModel.AuthAppName ?? string.Empty;
        ctx.Items["user"] = user;
        ctx.Items["format"] = authModel?.AuthOutputFormat;
        await next();
    }
}