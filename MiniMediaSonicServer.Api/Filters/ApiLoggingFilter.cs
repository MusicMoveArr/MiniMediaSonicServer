using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MiniMediaSonicServer.Api.Filters;

public sealed class ApiLoggingFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        using MemoryStream bodyStream = new MemoryStream();
        await context.HttpContext.Request.Body.CopyToAsync(bodyStream);

        string[] ignoreQueries = ["u", "p", "t", "s"];
        string bodyText = Encoding.UTF8.GetString(bodyStream.ToArray());
        string query = string.Join(", ", context.HttpContext.Request.Query
            .Where(query => !ignoreQueries.Contains(query.Key))
            .Select(query => $"{query.Key}='{query.Value}'"));
        
        Stopwatch sw = Stopwatch.StartNew();
        await next();
        sw.Stop();
        Console.WriteLine($"Request: {context.HttpContext.Request.Path}, Query: {query}, Body: {bodyText}, Response time: {sw.ElapsedMilliseconds}msec");
    }
}