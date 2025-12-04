using SchedulePlanner.Domain.Entities;
using SchedulePlanner.Domain.Interfaces;

namespace SchedulePlanner.Application.DTO;

public class Dto
{
    public record UserDto(Guid Id, string Username, string Email);
    public record CreateUserDto(string Username, string Email);

    public record CategoryDto(Guid Id, string Name, string Color);
    public record CreateCategoryDto(string Name, string Color);

    public record TaskDto(Guid Id, string Title, string Description, DateTime DueDate, string CategoryName, string UserName);

    public record CreateTaskDto(string Title, string Description, DateTime DueDate, Guid UserId, Guid CategoryId);
    public interface IHttpService
    {
        Task<T> SendGetRequestAsync<T>(string url);
    }
    
    public record ExternalTodoDto(int Id, int UserId, string Title, bool Completed);
    
    public class UserService {
        private readonly IUserRepository _userRepo;
        public UserService(IUserRepository userRepo) => _userRepo = userRepo;
        public async Task<UserDto> CreateUserAsync(CreateUserDto dto) {
             var user = new User { Id = Guid.NewGuid(), Username = dto.Username, Email = dto.Email };
             await _userRepo.AddAsync(user);
             return new UserDto(user.Id, user.Username, user.Email);
        }
        public async Task<List<UserDto>> GetAllUsersAsync() {
            var users = await _userRepo.GetAllAsync();
            return users.Select(u => new UserDto(u.Id, u.Username, u.Email)).ToList();
        }
    }

    public class CategoryService {
        private readonly ICategoryRepository _repo;
        public CategoryService(ICategoryRepository repo) => _repo = repo;
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto) {
            var category = new Category { Id = Guid.NewGuid(), Name = dto.Name, ColorHex = dto.Color };
            await _repo.AddAsync(category);
            return new CategoryDto(category.Id, category.Name, category.ColorHex);
        }
        public async Task<List<CategoryDto>> GetAllAsync() {
             var items = await _repo.GetAllAsync();
             return items.Select(c => new CategoryDto(c.Id, c.Name, c.ColorHex)).ToList();
        }
    }

    public class TaskService {
        private readonly ITaskRepository _taskRepo;
        private readonly IUserRepository _userRepo;
        private readonly ICategoryRepository _categoryRepo;
        public TaskService(ITaskRepository taskRepo, IUserRepository userRepo, ICategoryRepository categoryRepo) {
            _taskRepo = taskRepo; _userRepo = userRepo; _categoryRepo = categoryRepo;
        }
        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto) {
            var user = await _userRepo.GetByIdAsync(dto.UserId);
            if (user == null) throw new Exception("Пользователь не найден");
            var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);
            if (category == null) throw new Exception("Категория не найдена");
            var task = new ScheduleTask {
                Id = Guid.NewGuid(), Title = dto.Title, Description = dto.Description, DueDate = dto.DueDate, UserId = dto.UserId, CategoryId = dto.CategoryId
            };
            await _taskRepo.AddAsync(task);
            return new TaskDto(task.Id, task.Title, task.Description, task.DueDate, category.Name, user.Username);
        }
        public async Task<List<TaskDto>> GetUserTasksAsync(Guid userId) {
            var tasks = await _taskRepo.GetTasksByUserAsync(userId);
            return tasks.Select(t => new TaskDto(t.Id, t.Title, t.Description, t.DueDate, t.Category?.Name ?? "Без категории", t.User?.Username ?? "Неизвестный")).ToList();
        }
    }
}