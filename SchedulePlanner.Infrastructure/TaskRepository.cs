using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Domain.Entities;
using SchedulePlanner.Domain.Interfaces;

namespace SchedulePlanner.Infrastructure;

public class TaskRepository : ITaskRepository
{
    private readonly PlannerDbContext _context;
    public TaskRepository(PlannerDbContext context) => _context = context;

    public async Task AddAsync(ScheduleTask task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ScheduleTask>> GetTasksByUserAsync(Guid userId)
    {
        return await _context.Tasks
            .Include(t => t.Category)
            .Include(t => t.User)
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public Task<ScheduleTask?> GetByIdWithDetailsAsync(Guid id) => throw new NotImplementedException();
    public Task UpdateAsync(ScheduleTask task) => throw new NotImplementedException();
    public Task DeleteAsync(Guid id) => throw new NotImplementedException();
}