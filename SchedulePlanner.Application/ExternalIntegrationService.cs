using SchedulePlanner.Application.DTO;
using SchedulePlanner.Domain.Interfaces;

namespace SchedulePlanner.Application;

public class ExternalIntegrationService
{
    private readonly IHttpService _httpService;
    private readonly IRpcClient _rpcClient;

    public ExternalIntegrationService(IHttpService httpService, IRpcClient rpcClient)
    {
        _httpService = httpService;
        _rpcClient = rpcClient;
    }

    public async Task<Dto.ExternalTodoDto> GetInfoFromServiceB()
    {
        // string serviceB_Url = "https://localhost:7206/";
        string serviceB_Url = "https://jsonplaceholder.typicode.com/todos/1";
        var result = await _httpService.SendGetRequestAsync<Dto.ExternalTodoDto>(serviceB_Url);
            
        return result;
    }
    
    public async Task<string> TestRpcAsync(string message)
    {
        return await _rpcClient.CallAsync(message);
    }
}