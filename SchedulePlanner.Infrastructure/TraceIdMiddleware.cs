namespace SchedulePlanner.Infrastructure;

public class TraceIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string TraceIdHeaderName = "X-Trace-Id";

    public TraceIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(TraceIdHeaderName, out var traceId))
        {
            traceId = Guid.NewGuid().ToString();
        }

        context.Items[TraceIdHeaderName] = traceId.ToString();
        context.Response.Headers[TraceIdHeaderName] = traceId.ToString();

        await _next(context);
    }
}