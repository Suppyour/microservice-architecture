namespace SchedulePlanner.Domain.Entities;

public class CategoryEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<EventEntity> Events { get; set; } = new();
}