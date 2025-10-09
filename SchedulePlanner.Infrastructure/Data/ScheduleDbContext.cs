using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Domain.Entities;

namespace SchedulePlanner.Infrastructure.Data;

public class ScheduleDbContext : DbContext
{
    public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options)
        : base(options) { }

    public DbSet<EventEntity> Events => Set<EventEntity>();
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
}