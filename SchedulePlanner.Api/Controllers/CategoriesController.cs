using Microsoft.AspNetCore.Mvc;
using SchedulePlanner.Application.DTO;
using SchedulePlanner.Application.Services;

namespace SchedulePlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _service;

    public CategoriesController(CategoryService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Dto.CreateCategoryDto dto)
    {
        var result = await _service.CreateCategoryAsync(dto);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }
}
