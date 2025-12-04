using SchedulePlanner.Application;

namespace SchedulePlanner.Infrastructure;

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string TraceIdHeaderName = "X-Trace-Id";

    public HttpService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<T> SendGetRequestAsync<T>(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var context = _httpContextAccessor.HttpContext;

        if (context != null && context.Items.TryGetValue(TraceIdHeaderName, out var traceId))
        {
            if (!request.Headers.Contains(TraceIdHeaderName))
            {
                request.Headers.Add(TraceIdHeaderName, traceId.ToString());
            }
        }

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<T>();
        if (result == null) throw new Exception("Пусто");
            
        return result;
    }
}