namespace SchedulePlanner.Domain.Entities;

public class ScheduleTask
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}