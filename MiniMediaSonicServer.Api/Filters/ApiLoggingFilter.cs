using System.Security.Cryptography;
using System.Text;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Repositories;
using Aes = System.Security.Cryptography.Aes;

namespace MiniMediaSonicServer.Api.Filters;

public sealed class ApiLoggingFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        using MemoryStream bodyStream = new MemoryStream();
        await context.HttpContext.Request.Body.CopyToAsync(bodyStream);

        string[] ignoreQueries = ["u", "p", "t", "s"];
        string bodyText = Encoding.UTF8.GetString(bodyStream.ToArray());
        string query = string.Join('\n', context.HttpContext.Request.Query
            .Where(query => !ignoreQueries.Contains(query.Key))
            .Select(query => $"\t{query.Key}={query.Value}"));
        
        Console.WriteLine($"Request: {context.HttpContext.Request.Path}, Query: \n{query}, Body: {bodyText}");
        
        await next();
    }
}