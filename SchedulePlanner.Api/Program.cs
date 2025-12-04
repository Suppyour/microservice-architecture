using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Application;
using SchedulePlanner.Application.DTO;
using SchedulePlanner.Application.Services;
using SchedulePlanner.Domain.Interfaces;
using SchedulePlanner.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 1. Инфраструктура БД
builder.Services.AddDbContext<PlannerDbContext>(options =>
    options.UseInMemoryDatabase("PlannerDb"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// 2. HTTP Инфраструктура (НОВОЕ)
// IHttpContextAccessor нужен, чтобы HttpService мог читать текущий TraceId
builder.Services.AddHttpContextAccessor(); 

// Регистрируем типизированный HttpClient для HttpService
builder.Services.AddHttpClient<IHttpService, HttpService>();

// 3. Application Logic
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<ExternalIntegrationService>(); // Наш новый сервис

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. Подключаем Middleware (ВАЖНО: Порядок имеет значение)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// TraceIdMiddleware должен быть как можно раньше, чтобы поймать запрос первым
app.UseMiddleware<TraceIdMiddleware>();

app.MapControllers();
app.Run();