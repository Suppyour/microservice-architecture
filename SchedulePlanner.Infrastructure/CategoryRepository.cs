using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Domain.Entities;
using SchedulePlanner.Domain.Interfaces;

namespace SchedulePlanner.Infrastructure;

public class CategoryRepository : ICategoryRepository
{
    private readonly PlannerDbContext _context;
    public CategoryRepository(PlannerDbContext context) => _context = context;

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Category>> GetAllAsync() => await _context.Categories.ToListAsync();
    public async Task<Category?> GetByIdAsync(Guid id) => await _context.Categories.FindAsync(id);
    public Task DeleteAsync(Guid id) => throw new NotImplementedException();
}