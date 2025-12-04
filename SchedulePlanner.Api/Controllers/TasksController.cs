using Microsoft.AspNetCore.Mvc;
using SchedulePlanner.Application.DTO;
using SchedulePlanner.Application.Services;

namespace SchedulePlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TaskService _service;
    public TasksController(TaskService service) => _service = service;
    
    [HttpPost]
    public async Task<IActionResult> Create(Dto.CreateTaskDto dto)
    {
        try
        {
            var result = await _service.CreateTaskAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var result = await _service.GetUserTasksAsync(userId);
        return Ok(result);
    }
}