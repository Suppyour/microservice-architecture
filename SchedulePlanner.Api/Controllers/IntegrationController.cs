using Microsoft.AspNetCore.Mvc;
using SchedulePlanner.Application;
using SchedulePlanner.Application.DTO;

namespace SchedulePlanner.Api.Controllers;

[ApiController]
    [Route("api/[controller]")]
    public class IntegrationController : ControllerBase
    {
        private readonly ExternalIntegrationService _integrationService;

        public IntegrationController(ExternalIntegrationService integrationService)
        {
            _integrationService = integrationService;
        }

        [HttpGet("test-sync-call")]
        public async Task<IActionResult> TestSyncCall()
        {
            var data = await _integrationService.GetInfoFromServiceB();
            var traceId = HttpContext.Items["X-Trace-Id"];

            return Ok(new { 
                Message = "Данные успешно получены от внешнего сервиса (Service B)",
                TraceId = traceId,
                ExternalData = data 
            });
        }
        
        [HttpGet("rpc-test")]
        public async Task<IActionResult> TestRpc([FromQuery] string message = "ping")
        {
            var response = await _integrationService.TestRpcAsync(message);
            var traceId = HttpContext.Items["X-Trace-Id"];

            return Ok(new
            {
                Message = "RPC call successful.",
                TraceId = traceId,
                RpcResponse = response
            });
        }
    }