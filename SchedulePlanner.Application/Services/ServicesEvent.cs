using SchedulePlanner.Application.DTO;
using SchedulePlanner.Application.Interfaces;
using SchedulePlanner.Domain.Entities;

namespace SchedulePlanner.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _repo;

    public EventService(IEventRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<EventResponse>> GetAllAsync()
    {
        var events = await _repo.GetAllAsync();
        return events.Select(e => new EventResponse
        {
            Id = e.Id,
            Title = e.Title,
            CategoryName = e.Category?.Name ?? "Без категории"
        }).ToList();
    }

    public async Task<EventResponse?> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? null : new EventResponse
        {
            Id = e.Id,
            Title = e.Title,
            CategoryName = e.Category?.Name ?? "Без категории"
        };
    }

    public async Task<EventResponse> CreateAsync(EventRequest request)
    {
        var entity = new EventEntity
        {
            Title = request.Title,
            Description = request.Description,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };
        var result = await _repo.AddAsync(entity);

        return new EventResponse
        {
            Id = result.Id,
            Title = result.Title
        };
    }

    public async Task<EventResponse> UpdateAsync(int id, EventRequest request)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) throw new Exception("Событие не найдено");

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.StartTime = request.StartTime;
        entity.EndTime = request.EndTime;

        var updated = await _repo.UpdateAsync(entity);

        return new EventResponse
        {
            Id = updated.Id,
            Title = updated.Title
        };
    }

    public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);
}
