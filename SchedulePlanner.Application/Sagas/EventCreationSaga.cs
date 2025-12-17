using System;
using MassTransit;
using SchedulePlanner.Application.Contracts;

namespace SchedulePlanner.Application.Sagas;

public class EventCreationState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;

    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public Guid CategoryId { get; set; }
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class EventCreationSaga : MassTransitStateMachine<EventCreationState>
{
    public EventCreationSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => SubmitEventCreation, x => x.CorrelateById(m => m.Message.EventId));
        Event(() => UserValidated, x => x.CorrelateById(m => m.Message.EventId));
        Event(() => UserValidationFailed, x => x.CorrelateById(m => m.Message.EventId));
        Event(() => TimeSlotReserved, x => x.CorrelateById(m => m.Message.EventId));
        Event(() => TimeSlotReservationFailed, x => x.CorrelateById(m => m.Message.EventId));
        Event(() => TaskCreated, x => x.CorrelateById(m => m.Message.EventId));
        Event(() => TaskCreationFailed, x => x.CorrelateById(m => m.Message.EventId));
        Event(() => NotificationSent, x => x.CorrelateById(m => m.Message.EventId));

        Initially(
            When(SubmitEventCreation)
                .Then(context =>
                {
                    context.Saga.UserId = context.Message.UserId;
                    context.Saga.Title = context.Message.Title;
                    context.Saga.Description = context.Message.Description;
                    context.Saga.DueDate = context.Message.DueDate;
                    context.Saga.CategoryId = context.Message.CategoryId;
                    
                    // Simple logic: Event lasts 1 hour ending at due date
                    context.Saga.EndTime = context.Message.DueDate;
                    context.Saga.StartTime = context.Message.DueDate.AddHours(-1);
                })
                .TransitionTo(ValidatingUser)
                .Publish(context => new ValidateUserMessage(context.Saga.CorrelationId, context.Saga.UserId))
        );

        During(ValidatingUser,
            When(UserValidated)
                .TransitionTo(ReservingTimeSlot)
                .Publish(context => new ReserveTimeSlotMessage(context.Saga.CorrelationId, context.Saga.StartTime, context.Saga.EndTime)),
            When(UserValidationFailed)
                .TransitionTo(Failed)
                .Finalize()
        );

        During(ReservingTimeSlot,
            When(TimeSlotReserved)
                .TransitionTo(CreatingTask)
                .Publish(context => new CreateTaskMessage(
                    context.Saga.CorrelationId, 
                    context.Saga.UserId, 
                    context.Saga.Title, 
                    context.Saga.Description, 
                    context.Saga.DueDate, 
                    context.Saga.CategoryId)),
            When(TimeSlotReservationFailed)
                .TransitionTo(Failed)
                .Finalize()
        );

        During(CreatingTask,
            When(TaskCreated)
                .TransitionTo(SendingNotification)
                .Publish(context => new SendNotificationMessage(context.Saga.CorrelationId, context.Saga.UserId, $"Event '{context.Saga.Title}' created successfully.")),
            When(TaskCreationFailed)
                .TransitionTo(Failed)
                .Finalize()
        );

        During(SendingNotification,
            When(NotificationSent)
                .TransitionTo(Completed)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    public State ValidatingUser { get; private set; }
    public State ReservingTimeSlot { get; private set; }
    public State CreatingTask { get; private set; }
    public State SendingNotification { get; private set; }
    public State Completed { get; private set; }
    public State Failed { get; private set; }

    public Event<ISubmitEventCreation> SubmitEventCreation { get; private set; }
    public Event<IUserValidated> UserValidated { get; private set; }
    public Event<IUserValidationFailed> UserValidationFailed { get; private set; }
    public Event<ITimeSlotReserved> TimeSlotReserved { get; private set; }
    public Event<ITimeSlotReservationFailed> TimeSlotReservationFailed { get; private set; }
    public Event<ITaskCreated> TaskCreated { get; private set; }
    public Event<ITaskCreationFailed> TaskCreationFailed { get; private set; }
    public Event<INotificationSent> NotificationSent { get; private set; }
}

public record ValidateUserMessage(Guid EventId, Guid UserId) : IValidateUser;
public record ReserveTimeSlotMessage(Guid EventId, DateTime StartTime, DateTime EndTime) : IReserveTimeSlot;
public record CreateTaskMessage(Guid EventId, Guid UserId, string Title, string Description, DateTime DueDate, Guid CategoryId) : ICreateTask;
public record SendNotificationMessage(Guid EventId, Guid UserId, string Message) : ISendNotification;
