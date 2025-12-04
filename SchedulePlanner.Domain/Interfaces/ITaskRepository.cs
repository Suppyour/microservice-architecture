using SchedulePlanner.Domain.Entities;

namespace SchedulePlanner.Domain.Interfaces;

public interface ITaskRepository
{
    Task<ScheduleTask?> GetByIdWithDetailsAsync(Guid id);
    Task<List<ScheduleTask>> GetTasksByUserAsync(Guid userId);
    Task AddAsync(ScheduleTask task);
    Task UpdateAsync(ScheduleTask task);
    Task DeleteAsync(Guid id);
}