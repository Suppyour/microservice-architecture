namespace SchedulePlanner.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#FFFFFF";

    public ICollection<ScheduleTask> Tasks { get; set; } = new List<ScheduleTask>();
}