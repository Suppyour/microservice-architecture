namespace SchedulePlanner.Application;

public interface IHttpService
{
    Task<T> SendGetRequestAsync<T>(string url);
}