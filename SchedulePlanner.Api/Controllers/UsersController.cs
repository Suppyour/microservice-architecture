using Microsoft.AspNetCore.Mvc;
using SchedulePlanner.Application.DTO;
using SchedulePlanner.Application.Services;

namespace SchedulePlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _service;
    public UsersController(UserService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create(Dto.CreateUserDto dto)
    {
        var result = await _service.CreateUserAsync(dto);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllUsersAsync());
}