using SchedulePlanner.Domain.Entities;

namespace SchedulePlanner.Application.Interfaces;

public interface IEventRepository
{
    Task<List<EventEntity>> GetAllAsync();
    Task<EventEntity?> GetByIdAsync(int id);
    Task<EventEntity> AddAsync(EventEntity entity);
    Task<EventEntity> UpdateAsync(EventEntity entity);
    Task DeleteAsync(int id);
}