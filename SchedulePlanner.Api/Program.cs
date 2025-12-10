using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Application;
using SchedulePlanner.Application.Services;
using SchedulePlanner.Application.Interfaces;
using SchedulePlanner.Domain.Interfaces;
using SchedulePlanner.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchedulePlanner.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PlannerDbContext>(options =>
    options.UseInMemoryDatabase("PlannerDb"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();


builder.Services.AddHttpContextAccessor(); 

builder.Services.AddHttpClient<IHttpService, HttpService>();

builder.Services.AddSingleton<IRpcClient, RpcClient>();
builder.Services.AddHostedService<RpcServer>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ExternalIntegrationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<TraceIdMiddleware>();

app.MapControllers();
app.Run();