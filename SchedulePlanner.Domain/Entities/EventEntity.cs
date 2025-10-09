namespace SchedulePlanner.Domain.Entities;

public class EventEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int CategoryId { get; set; }
    public CategoryEntity? Category { get; set; }
}