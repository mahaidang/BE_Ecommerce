using System.Net;
using System.Text.Json;

namespace ApiGateway.Api.Middlewares;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _log;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> log)
    {
        _next = next;
        _log = log;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            var cid = ctx.Items[CorrelationIdMiddleware.HeaderName]?.ToString();
            _log.LogError(ex, "❌ Error: {Message} | cid={Cid}", ex.Message, cid);

            ctx.Response.ContentType = "application/problem+json";
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var problem = new
            {
                type = "https://httpstatuses.com/500",
                title = "Internal Server Error",
                status = 500,
                correlationId = cid,
                traceId = ctx.TraceIdentifier,
                detail = ex.Message
            };

            await ctx.Response.WriteAsync(JsonSerializer.Serialize(problem,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
    }
}
