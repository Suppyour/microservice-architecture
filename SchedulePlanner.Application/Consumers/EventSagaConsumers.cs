using MassTransit;
using SchedulePlanner.Application.Contracts;
using SchedulePlanner.Application.DTO;
using SchedulePlanner.Application.Services;
using SchedulePlanner.Domain.Interfaces;
using SchedulePlanner.Application.Interfaces;

namespace SchedulePlanner.Application.Consumers;

public class ValidateUserConsumer : IConsumer<IValidateUser>
{
    private readonly IUserRepository _userRepo;

    public ValidateUserConsumer(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task Consume(ConsumeContext<IValidateUser> context)
    {
        var user = await _userRepo.GetByIdAsync(context.Message.UserId);
        if (user != null)
        {
            await context.Publish<IUserValidated>(new { context.Message.EventId, context.Message.UserId });
        }
        else
        {
            await context.Publish<IUserValidationFailed>(new { context.Message.EventId, context.Message.UserId, Reason = "User not found" });
        }
    }
}

public class ReserveTimeSlotConsumer : IConsumer<IReserveTimeSlot>
{
    
    public async Task Consume(ConsumeContext<IReserveTimeSlot> context)
    {
        if (context.Message.StartTime > DateTime.UtcNow)
        {
            await context.Publish<ITimeSlotReserved>(new { context.Message.EventId });
        }
        else
        {
            await context.Publish<ITimeSlotReservationFailed>(new { context.Message.EventId, Reason = "Start time is in the past" });
        }
    }
}

public class CreateTaskConsumer : IConsumer<ICreateTask>
{
    private readonly ITaskService _taskService;

    public CreateTaskConsumer(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public async Task Consume(ConsumeContext<ICreateTask> context)
    {
        try
        {
            var dto = new Dto.CreateTaskDto(
                context.Message.Title,
                context.Message.Description,
                context.Message.DueDate,
                context.Message.UserId,
                context.Message.CategoryId
            );
            var createdTask = await _taskService.CreateTaskAsync(dto);
            
            await context.Publish<ITaskCreated>(new { context.Message.EventId, TaskId = createdTask.Id });
        }
        catch (Exception ex)
        {
            await context.Publish<ITaskCreationFailed>(new { context.Message.EventId, Reason = ex.Message });
        }
    }
}

public class SendNotificationConsumer : IConsumer<ISendNotification>
{
    public Task Consume(ConsumeContext<ISendNotification> context)
    {
        Console.WriteLine($"[NotificationService] Sending notification to User {context.Message.UserId}: {context.Message.Message}");
        
        return context.Publish<INotificationSent>(new { context.Message.EventId });
    }
}
