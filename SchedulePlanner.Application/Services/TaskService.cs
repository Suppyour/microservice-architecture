using SchedulePlanner.Application.DTO;
using SchedulePlanner.Domain.Entities;
using SchedulePlanner.Domain.Interfaces;

namespace SchedulePlanner.Application.Services;

public class TaskService
    {
        private readonly ITaskRepository _taskRepo;
        private readonly IUserRepository _userRepo;
        private readonly ICategoryRepository _categoryRepo;
        
        public TaskService(ITaskRepository taskRepo, IUserRepository userRepo, ICategoryRepository categoryRepo)
        {
            _taskRepo = taskRepo;
            _userRepo = userRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<Dto.TaskDto> CreateTaskAsync(Dto.CreateTaskDto dto)
        {
            var user = await _userRepo.GetByIdAsync(dto.UserId);
            if (user == null) throw new Exception("Пользователь не найден");

            var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);
            if (category == null) throw new Exception("Категория не найдена");
            
            if (dto.DueDate < DateTime.UtcNow) throw new Exception("Нельзя создать задачу в прошлом");

            var task = new ScheduleTask
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                UserId = dto.UserId,
                CategoryId = dto.CategoryId,
                IsCompleted = false
            };

            await _taskRepo.AddAsync(task);
            
            return new Dto.TaskDto(task.Id, task.Title, task.Description, task.DueDate, category.Name, user.Username);
        }

        public async Task<List<Dto.TaskDto>> GetUserTasksAsync(Guid userId)
        {
            var tasks = await _taskRepo.GetTasksByUserAsync(userId);
            
            return tasks.Select(t => new Dto.TaskDto(
                t.Id, 
                t.Title, 
                t.Description, 
                t.DueDate, 
                t.Category?.Name ?? "Без категории",
                t.User?.Username ?? "Неизвестный"
            )).ToList();
        }
    }