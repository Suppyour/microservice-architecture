using SchedulePlanner.Application.DTO;

namespace SchedulePlanner.Application.Interfaces
{
    public interface ITaskService
    {
        Task<Dto.TaskDto> CreateTaskAsync(Dto.CreateTaskDto dto);
        Task<List<Dto.TaskDto>> GetUserTasksAsync(Guid userId);
    }
}
