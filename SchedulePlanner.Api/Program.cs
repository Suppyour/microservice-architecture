using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Application.Interfaces;
using SchedulePlanner.Application.Services;
using SchedulePlanner.Infrastructure.Data;
using SchedulePlanner.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ScheduleDbContext>(opt =>
    opt.UseInMemoryDatabase("SchedulePlannerDb"));

builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();