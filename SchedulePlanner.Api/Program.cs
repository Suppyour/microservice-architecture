using MassTransit;
using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Application;
using SchedulePlanner.Application.Services;
using SchedulePlanner.Application.Interfaces;
using SchedulePlanner.Domain.Interfaces;
using SchedulePlanner.Infrastructure;
using SchedulePlanner.Api;
using SchedulePlanner.Application.Sagas;
using SchedulePlanner.Application.Consumers;

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

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ValidateUserConsumer>();
    x.AddConsumer<ReserveTimeSlotConsumer>();
    x.AddConsumer<CreateTaskConsumer>();
    x.AddConsumer<SendNotificationConsumer>();

    x.AddSagaStateMachine<EventCreationSaga, EventCreationState>()
        .InMemoryRepository();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

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