using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Application.Services; // Убедись, что namespace верный
using SchedulePlanner.Domain.Interfaces;    // Убедись, что namespace верный
using SchedulePlanner.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// 1. Настройка БД
builder.Services.AddDbContext<PlannerDbContext>(options =>
    options.UseInMemoryDatabase("PlannerDb"));

// 2. Регистрация зависимостей (DI)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<TaskService>();

builder.Services.AddControllers();

// --- ВОТ ЭТОГО НЕ ХВАТАЛО ---
// Регистрируем генератор Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// ----------------------------

var app = builder.Build();

// 3. Настройка Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Это создает страницу с UI
}

app.MapControllers();

app.Run();