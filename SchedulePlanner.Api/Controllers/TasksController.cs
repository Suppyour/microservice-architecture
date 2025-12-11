using Microsoft.AspNetCore.Mvc;
using SchedulePlanner.Application.DTO;
using SchedulePlanner.Application.Services;
using SchedulePlanner.Application.Interfaces; // <--- ОБЯЗАТЕЛЬНО добавить этот using

namespace SchedulePlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;
    
    public TasksController(ITaskService service) 
    {
        _service = service;
    }
    
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
    
    [HttpPost("saga")]
    public async Task<IActionResult> CreateSaga(Dto.CreateTaskDto dto, [FromServices] MassTransit.IPublishEndpoint publishEndpoint)
    {
        var eventId = Guid.NewGuid();
        await publishEndpoint.Publish<SchedulePlanner.Application.Contracts.ISubmitEventCreation>(new
        {
            EventId = eventId,
            UserId = dto.UserId,
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            CategoryId = dto.CategoryId
        });
        return Accepted(new { EventId = eventId, Message = "Event creation started via SAGA" });
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var result = await _service.GetUserTasksAsync(userId);
        return Ok(result);
    }
}