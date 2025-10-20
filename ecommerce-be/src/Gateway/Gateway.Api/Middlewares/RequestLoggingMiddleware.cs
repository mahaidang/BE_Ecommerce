using System.Diagnostics;

namespace ApiGateway.Api.Middlewares;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _log;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> log)
    {
        _next = next;
        _log = log;
    }

    public async Task Invoke(HttpContext ctx)
    {
        var sw = Stopwatch.StartNew();
        var cid = ctx.Items[CorrelationIdMiddleware.HeaderName]?.ToString();
        var method = ctx.Request.Method;
        var path = ctx.Request.Path;

        try
        {
            await _next(ctx);
            sw.Stop();
            _log.LogInformation("➡️ {Method} {Path} -> {Status} ({Elapsed} ms) cid={Cid}",
                method, path, ctx.Response.StatusCode, sw.ElapsedMilliseconds, cid);
        }
        catch (Exception ex)
        {
            sw.Stop();
            _log.LogError(ex, "❌ {Method} {Path} failed after {Elapsed} ms cid={Cid}",
                method, path, sw.ElapsedMilliseconds, cid);
            throw;
        }
    }
}
