using System;

namespace SchedulePlanner.Application.Contracts;

public interface ISubmitEventCreation
{
    Guid EventId { get; }
    Guid UserId { get; }
    string Title { get; }
    string Description { get; }
    DateTime DueDate { get; }
    Guid CategoryId { get; }
}

public interface IValidateUser
{
    Guid EventId { get; }
    Guid UserId { get; }
}

public interface IUserValidated
{
    Guid EventId { get; }
    Guid UserId { get; }
}

public interface IUserValidationFailed
{
    Guid EventId { get; }
    Guid UserId { get; }
    string Reason { get; }
}

public interface IReserveTimeSlot
{
    Guid EventId { get; }
    DateTime StartTime { get; }
    DateTime EndTime { get; }
}

public interface ITimeSlotReserved
{
    Guid EventId { get; }
}

public interface ITimeSlotReservationFailed
{
    Guid EventId { get; }
    string Reason { get; }
}

public interface ICreateTask
{
    Guid EventId { get; }
    Guid UserId { get; }
    string Title { get; }
    string Description { get; }
    DateTime DueDate { get; }
    Guid CategoryId { get; }
}

public interface ITaskCreated
{
    Guid EventId { get; }
    Guid TaskId { get; }
}

public interface ITaskCreationFailed
{
    Guid EventId { get; }
    string Reason { get; }
}

public interface ISendNotification
{
    Guid EventId { get; }
    Guid UserId { get; }
    string Message { get; }
}

public interface INotificationSent
{
    Guid EventId { get; }
}
