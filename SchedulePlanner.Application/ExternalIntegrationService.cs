using SchedulePlanner.Application.DTO;

namespace SchedulePlanner.Application;

public class ExternalIntegrationService
{
    private readonly IHttpService _httpService;

    public ExternalIntegrationService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<Dto.ExternalTodoDto> GetInfoFromServiceB()
    {
        // string serviceB_Url = "https://localhost:7206/";
        string serviceB_Url = "https://jsonplaceholder.typicode.com/todos/1";
        var result = await _httpService.SendGetRequestAsync<Dto.ExternalTodoDto>(serviceB_Url);
            
        return result;
    }
}