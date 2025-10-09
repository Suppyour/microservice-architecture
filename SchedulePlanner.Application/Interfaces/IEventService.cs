using SchedulePlanner.Application.DTO;

namespace SchedulePlanner.Application.Interfaces;

public interface IEventService
{
    Task<List<EventResponse>> GetAllAsync();
    Task<EventResponse?> GetByIdAsync(int id);
    Task<EventResponse> CreateAsync(EventRequest request);
    Task<EventResponse> UpdateAsync(int id, EventRequest request);
    Task DeleteAsync(int id);
}