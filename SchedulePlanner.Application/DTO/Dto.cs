namespace SchedulePlanner.Application.DTO;

public class Dto
{
    public record UserDto(Guid Id, string Username, string Email);
    public record CreateUserDto(string Username, string Email);

    public record CategoryDto(Guid Id, string Name, string Color);
    public record CreateCategoryDto(string Name, string Color);

    public record TaskDto(Guid Id, string Title, string Description, DateTime DueDate, string CategoryName, string UserName);

    public record CreateTaskDto(string Title, string Description, DateTime DueDate, Guid UserId, Guid CategoryId);
    
    public record ExternalTodoDto(int Id, int UserId, string Title, bool Completed);
}
