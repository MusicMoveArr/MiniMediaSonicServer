using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic;

public static class SubsonicResults
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = null,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public static IResult Ok(HttpContext ctx, SubsonicResponse response)
    {
        return Write(ctx, response);
    }

    public static IResult Fail(HttpContext ctx, int code, string message)
    {
        var r = new SubsonicResponse
        {
            Status = "failed",
            Error = new Error { Code = code, Message = message }
        };
        return Write(ctx, r);
    }

    private static IResult Write(HttpContext ctx, SubsonicResponse response)
    {
        var f = (ctx.Request.Query["f"].FirstOrDefault() ?? "").ToLowerInvariant();

        if (f == "json" || f == "jsonp")
        {
            var env = new SubsonicEnvelope { Response = response };
            return Results.Json(env, JsonOptions);
        }

        // Default: XML
        var xml = SerializeXml(response);
        return Results.Text(xml, "text/xml; charset=utf-8");
    }

    public static ActionResult OkActionResult(HttpContext ctx, SubsonicResponse response)
    {
        return WriteActionResult(ctx, response);
    }

    public static ActionResult FailActionResult(HttpContext ctx, int code, string message)
    {
        var r = new SubsonicResponse
        {
            Status = "failed",
            Error = new Error { Code = code, Message = message }
        };
        return WriteActionResult(ctx, r);
    }

    private static ActionResult WriteActionResult(HttpContext ctx, SubsonicResponse response)
    {
        var f = (ctx.Request.Query["f"].FirstOrDefault() ?? "").ToLowerInvariant();

        if (f == "json" || f == "jsonp")
        {
            var env = new SubsonicEnvelope { Response = response };
            
            return new JsonResult(env, JsonOptions)
            {
                StatusCode = StatusCodes.Status200OK,
                ContentType = "application/json"
            };
        }

        return new ContentResult
        {
            ContentType = "text/xml; charset=utf-8",
            Content = SerializeXml(response),
            StatusCode = 200
        };
    }

    private static string SerializeXml(SubsonicResponse response)
    {
        var serializer = new XmlSerializer(typeof(SubsonicResponse));
        var ns = new XmlSerializerNamespaces();
        ns.Add(string.Empty, "http://subsonic.org/restapi");

        var settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = false,
            Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            Indent = false
        };

        using var sw = new StringWriterWithEncoding(settings.Encoding);
        using var xw = XmlWriter.Create(sw, settings);
        serializer.Serialize(xw, response, ns);
        return sw.ToString();
    }

    private sealed class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding _enc;
        public StringWriterWithEncoding(Encoding enc) => _enc = enc;
        public override Encoding Encoding => _enc;
    }
}