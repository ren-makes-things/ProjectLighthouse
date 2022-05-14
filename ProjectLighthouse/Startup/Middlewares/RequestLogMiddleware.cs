using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Logging;
using Microsoft.AspNetCore.Http;

namespace LBPUnion.ProjectLighthouse.Startup.Middlewares;

public class RequestLogMiddleware
{
    private readonly RequestDelegate next;

    public RequestLogMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    // Logs every request and the response to it
    // Example: "200, 13ms: GET /LITTLEBIGPLANETPS3_XML/news"
    // Example: "404, 127ms: GET /asdasd?query=osucookiezi727ppbluezenithtopplayhdhr"
    public async Task InvokeAsync(HttpContext context)
    {
        Stopwatch requestStopwatch = new();
        requestStopwatch.Start();

        context.Request.EnableBuffering(); // Allows us to reset the position of Request.Body for later logging

        // Log all headers.
//                    foreach (KeyValuePair<string, StringValues> header in context.Request.Headers) Logger.Log($"{header.Key}: {header.Value}");

        await next(context); // Handle the request so we can get the status code from it

        requestStopwatch.Stop();

        Logger.LogInfo
        (
            $"{context.Response.StatusCode}, {requestStopwatch.ElapsedMilliseconds}ms: {context.Request.Method} {context.Request.Path}{context.Request.QueryString}",
            LogArea.HTTP
        );

        #if DEBUG
        // Log post body
        if (context.Request.Method == "POST")
        {
            context.Request.Body.Position = 0;
            Logger.LogDebug(await new StreamReader(context.Request.Body).ReadToEndAsync(), LogArea.HTTP);
        }
        #endif
    }
}