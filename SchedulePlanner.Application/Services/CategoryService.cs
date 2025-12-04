using SchedulePlanner.Application.DTO;
using SchedulePlanner.Domain.Entities;
using SchedulePlanner.Domain.Interfaces;

namespace SchedulePlanner.Application.Services;

public class CategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<Dto.CategoryDto> CreateCategoryAsync(Dto.CreateCategoryDto dto)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            ColorHex = dto.Color
        };

        await _repo.AddAsync(category);
        return new Dto.CategoryDto(category.Id, category.Name, category.ColorHex);
    }
        
    public async Task<List<Dto.CategoryDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(c => new Dto.CategoryDto(c.Id, c.Name, c.ColorHex)).ToList();
    }
}