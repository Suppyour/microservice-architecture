using System;

namespace SchedulePlanner.Application.Contracts;

// --- Entry Point ---
public interface ISubmitEventCreation
{
    Guid EventId { get; }
    Guid UserId { get; }
    string Title { get; }
    string Description { get; }
    DateTime DueDate { get; }
    Guid CategoryId { get; }
}

// --- Step 1: User Validation ---
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

// --- Step 2: Time Slot Reservation ---
public interface IReserveTimeSlot
{
    Guid EventId { get; }
    DateTime StartTime { get; } // Assumed logic uses DueDate as start or similar
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

// --- Step 3: Create Task (Persistence) ---
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
    Guid TaskId { get; } // ID of the created task entity
}

public interface ITaskCreationFailed
{
    Guid EventId { get; }
    string Reason { get; }
}

// --- Step 4: Notification ---
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
