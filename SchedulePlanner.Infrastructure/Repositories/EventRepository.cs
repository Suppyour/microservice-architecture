using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Application.Interfaces;
using SchedulePlanner.Domain.Entities;
using SchedulePlanner.Infrastructure.Data;

namespace SchedulePlanner.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly ScheduleDbContext _db;

    public EventRepository(ScheduleDbContext db)
    {
        _db = db;
    }

    public async Task<List<EventEntity>> GetAllAsync() =>
        await _db.Events.Include(e => e.Category).ToListAsync();

    public async Task<EventEntity?> GetByIdAsync(int id) =>
        await _db.Events.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id);

    public async Task<EventEntity> AddAsync(EventEntity entity)
    {
        _db.Events.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<EventEntity> UpdateAsync(EventEntity entity)
    {
        _db.Events.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _db.Events.FindAsync(id);
        if (existing != null)
        {
            _db.Events.Remove(existing);
            await _db.SaveChangesAsync();
        }
    }
}