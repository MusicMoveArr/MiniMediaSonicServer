using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using Microsoft.AspNetCore.Mvc.Filters;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
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
        
        SubsonicAuthModel authModel = new  SubsonicAuthModel();
        authModel.Username = q["u"].FirstOrDefault() ?? string.Empty;
        authModel.Password = q["p"].FirstOrDefault() ?? string.Empty;
        authModel.Token = q["t"].FirstOrDefault() ?? string.Empty;
        authModel.Salt = q["s"].FirstOrDefault() ?? string.Empty;
        authModel.AppName = q["c"].FirstOrDefault() ?? string.Empty;

        try
        {
            //not always works atm somehow
            if (context.HttpContext.Request.Body.Length > 0)
            {
                var request = context.HttpContext.Request;
                request.Body.Position = 0;
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                if (!string.IsNullOrWhiteSpace(body))
                {
                    var model = JsonSerializer.Deserialize<SubsonicAuthModel>(body);
                    authModel.Username = !string.IsNullOrWhiteSpace(model.Username) ? model.Username : authModel.Username;
                    authModel.Password = !string.IsNullOrWhiteSpace(model.Password) ? model.Password : authModel.Password;
                    authModel.Token = !string.IsNullOrWhiteSpace(model.Token) ? model.Token : authModel.Token;
                    authModel.Salt = !string.IsNullOrWhiteSpace(model.Salt) ? model.Salt : authModel.Salt;
                    authModel.AppName = !string.IsNullOrWhiteSpace(model.AppName) ? model.AppName : authModel.AppName;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e + "\r\n" + e.StackTrace);
        }

        if (string.IsNullOrWhiteSpace(authModel.Username))
        {
            context.Result = SubsonicResults.FailActionResult(ctx, 10, "Required parameter is missing");
            return;
        }
        if (!Regex.IsMatch(authModel.Username, "^[a-zA-Z0-9_-]+$", RegexOptions.None, TimeSpan.FromMilliseconds(100)))
        {
            context.Result = SubsonicResults.FailActionResult(ctx, 40, "Wrong username or password");
            return;
        }

        var authenticated = false;
        var user = await _userService.GetUserByUsernameAsync(authModel.Username);
        
        if (user == null)
        {
            context.Result = SubsonicResults.FailActionResult(ctx, 40, "Wrong username or password");
            return;
        }
        
        if (!string.IsNullOrWhiteSpace(authModel.Token) && !string.IsNullOrWhiteSpace(authModel.Salt))
        {
            authenticated = _userService.ValidateToken(user.TokenBasedAuth, authModel.Token, authModel.Salt);
        }
        else if (!string.IsNullOrWhiteSpace(authModel.Password))
        {
            authenticated = _userService.ValidatePassword(authModel.Password, user.Password);
        }

        if (!authenticated)
        {
            context.Result = SubsonicResults.FailActionResult(ctx, 40, "Wrong username or password");
            return;
        }

        user.ClientName = authModel.AppName;
        ctx.Items["user"] = user;
        await next();
    }
}