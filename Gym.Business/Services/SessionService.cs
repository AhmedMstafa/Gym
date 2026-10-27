using Gym.Business.Common;
using Gym.Business.ViewModels;
using Gym.Business.ViewModels.Sessions;
using Gym.DataAccess.Entities;
using Gym.DataAccess.Queries;
using Gym.DataAccess.Repositories;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace Gym.Business.Services;

internal class SessionService(
    ISessionQueryService sessionQueryService,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IDateTimeProvider dateTimeProvider,
    ILogger<SessionService> logger) : ISessionService
{
    public async Task<IReadOnlyList<SessionIndexViewModel>> GetIndexItemsAsync(
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Retrieving session index items from database.");
        var sessions = await sessionQueryService.GetIndexItemsAsync(cancellationToken);
        var now = dateTimeProvider.Now;

        var result = sessions.Select(session => new SessionIndexViewModel
        {
            Id = session.Id,
            Speciality = session.Speciality,
            Description = session.Description,
            TrainerName = session.TrainerName,
            Capacity = session.Capacity,
            BookedCount = session.BookedCount,
            StartDate = session.StartDate,
            EndDate = session.EndDate,
            TrainerId = session.TrainerId,
            Status = GetStatus(session.StartDate, session.EndDate, now)
        }).ToList();

        logger.LogInformation("Loaded {Count} sessions successfully.", result.Count);
        return result;
    }

    public async Task<Result<SessionDetailsViewModel>> GetDetailsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Retrieving details for session ID {SessionId}.", id);

        var session = await unitOfWork.Sessions.GetByIdAsync(
            id: id,
            cancellationToken: cancellationToken,
            trackChanges: false,
            predicate: null,
            s => s.Category,
            s => s.Trainer,
            s => s.Bookings);

        if (session is null)
        {
            logger.LogWarning("Session details query failed: Session with ID {SessionId} was not found.", id);
            return Result.Failure<SessionDetailsViewModel>("SessionNotFound", $"Session with ID {id} was not found.");
        }

        var viewModel = mapper.Map<SessionDetailsViewModel>(session);
        var now = dateTimeProvider.Now;
        viewModel.Status = GetStatus(session.StartDate, session.EndDate, now);

        return Result.Success(viewModel);
    }

    public async Task<Result> CreateAsync(CreateSessionViewModel model, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Attempting to create session for category {CategoryId} with trainer {TrainerId}.", model.CategoryId, model.TrainerId);

        var validation = await ValidateSessionScheduleAsync(
            categoryId: model.CategoryId,
            trainerId: model.TrainerId,
            startDate: model.StartDate,
            endDate: model.EndDate,
            capacity: model.Capacity,
            minimumCapacity: 1,
            excludeSessionId: null,
            requireFutureStart: true,
            cancellationToken);

        if (validation.IsFailure)
            return validation;

        var session = mapper.Map<Session>(model);

        try
        {
            await unitOfWork.Sessions.AddAsync(session, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Session successfully created with ID {SessionId}.", session.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            var errorMsg = ex.InnerException?.Message ?? ex.Message;
            logger.LogError(ex, "Database error occurred while creating session.");
            return Result.Failure("DatabaseError", $"Database Error: {errorMsg}");
        }
    }

    public async Task<Result<EditSessionViewModel>> GetForUpdateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Retrieving session for update with ID {SessionId}.", id);

        var session = await unitOfWork.Sessions.GetByIdAsync(id, cancellationToken);

        if (session is null)
        {
            logger.LogWarning("Session update lookup failed: Session with ID {SessionId} was not found.", id);
            return Result.Failure<EditSessionViewModel>("SessionNotFound", $"Session with ID {id} was not found.");
        }

        if (GetStatus(session.StartDate, session.EndDate, dateTimeProvider.Now) != SessionStatus.Upcoming)
        {
            logger.LogWarning("Session update lookup failed: Session with ID {SessionId} is not upcoming.", id);
            return Result.Failure<EditSessionViewModel>(
                "SessionNotEditable",
                "Only upcoming sessions can be edited.");
        }

        return Result.Success(mapper.Map<EditSessionViewModel>(session));
    }

    public async Task<Result> UpdateAsync(
        int id,
        EditSessionViewModel model,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Updating session with ID {SessionId}.", id);

        var session = await unitOfWork.Sessions.GetByIdAsync(
            id: id,
            cancellationToken: cancellationToken,
            trackChanges: true,
            predicate: null,
            s => s.Bookings);

        if (session is null)
        {
            logger.LogWarning("Session update failed: Session with ID {SessionId} was not found.", id);
            return Result.Failure("SessionNotFound", $"Session with ID {id} was not found.");
        }

        if (GetStatus(session.StartDate, session.EndDate, dateTimeProvider.Now) != SessionStatus.Upcoming)
        {
            logger.LogWarning("Session update failed: Session with ID {SessionId} is not upcoming.", id);
            return Result.Failure("SessionNotEditable", "Only upcoming sessions can be edited.");
        }

        var bookedCount = session.Bookings?.Count ?? 0;
        var validation = await ValidateSessionScheduleAsync(
            categoryId: model.CategoryId,
            trainerId: model.TrainerId,
            startDate: model.StartDate,
            endDate: model.EndDate,
            capacity: model.Capacity,
            minimumCapacity: Math.Max(1, bookedCount),
            excludeSessionId: id,
            requireFutureStart: true,
            cancellationToken);

        if (validation.IsFailure)
            return validation;

        session.CategoryId = model.CategoryId;
        session.TrainerId = model.TrainerId;
        session.Description = model.Description.Trim();
        session.Capacity = model.Capacity;
        session.StartDate = model.StartDate;
        session.EndDate = model.EndDate;

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Session with ID {SessionId} was updated successfully.", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            var errorMsg = ex.InnerException?.Message ?? ex.Message;
            logger.LogError(ex, "Database error occurred while updating session {SessionId}.", id);
            return Result.Failure("DatabaseError", $"Database Error: {errorMsg}");
        }
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Attempting to delete session with ID {SessionId}.", id);

        var session = await unitOfWork.Sessions.GetByIdAsync(
            id: id,
            cancellationToken: cancellationToken,
            trackChanges: true,
            predicate: null,
            s => s.Bookings);

        if (session is null)
        {
            logger.LogWarning("Session deletion failed: Session with ID {SessionId} was not found.", id);
            return Result.Failure("SessionNotFound", $"Session with ID {id} was not found.");
        }

        if (session.Bookings is { Count: > 0 })
        {
            logger.LogWarning("Session deletion failed: Session with ID {SessionId} has active bookings.", id);
            return Result.Failure(
                "ActiveBookings",
                "This session has member bookings and cannot be deleted.");
        }

        await unitOfWork.Sessions.SoftDeleteAsync(session, cancellationToken);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Session with ID {SessionId} was soft deleted successfully.", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            var errorMsg = ex.InnerException?.Message ?? ex.Message;
            logger.LogError(ex, "Database error occurred while deleting session {SessionId}.", id);
            return Result.Failure("DatabaseError", $"Database Error: {errorMsg}");
        }
    }

    public async Task<IReadOnlyList<CategoryViewModel>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await unitOfWork.Categories.GetAllAsync(cancellationToken);
        return categories.Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name }).ToList();
    }

    public async Task<IReadOnlyList<TrainerViewModel>> GetTrainersAsync(CancellationToken cancellationToken = default)
    {
        var trainers = await unitOfWork.Trainers.GetAllAsync(cancellationToken);
        return trainers.Select(t => new TrainerViewModel { Id = t.Id, Name = t.Name, Speciality = t.Speciality }).ToList();
    }

    public async Task<IReadOnlyList<TrainerViewModel>> GetTrainersByCategoryAsync(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(
            categoryId,
            cancellationToken,
            trackChanges: false);

        if (category is null)
            return [];

        var trainers = await unitOfWork.Trainers.GetAllAsync(cancellationToken);
        return trainers
            .Where(t => t.Speciality == category.RequiredSpeciality)
            .Select(t => new TrainerViewModel { Id = t.Id, Name = t.Name, Speciality = t.Speciality })
            .ToList();
    }

    private async Task<Result> ValidateSessionScheduleAsync(
        int categoryId,
        int trainerId,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        int minimumCapacity,
        int? excludeSessionId,
        bool requireFutureStart,
        CancellationToken cancellationToken)
    {
        if (endDate <= startDate)
        {
            logger.LogWarning("Session validation failed: End date must be after start date.");
            return Result.Failure("EndDate", "End date and time must be after start date and time.");
        }

        if (requireFutureStart && startDate < dateTimeProvider.Now)
        {
            logger.LogWarning("Session validation failed: Start date is in the past.");
            return Result.Failure("StartDate", "Session start date and time cannot be in the past.");
        }

        if (capacity < minimumCapacity || capacity > 25)
        {
            logger.LogWarning("Session validation failed: Invalid capacity {Capacity}.", capacity);
            return minimumCapacity > 1
                ? Result.Failure("Capacity", $"Capacity cannot be less than the number of existing bookings ({minimumCapacity}).")
                : Result.Failure("Capacity", "Capacity must be between 1 and 25.");
        }

        var category = await unitOfWork.Categories.GetByIdAsync(
            categoryId,
            cancellationToken,
            trackChanges: false);

        if (category is null)
        {
            logger.LogWarning("Session validation failed: Category ID {CategoryId} does not exist.", categoryId);
            return Result.Failure("CategoryId", "Selected category does not exist.");
        }

        var trainer = await unitOfWork.Trainers.GetByIdAsync(trainerId, cancellationToken, trackChanges: false);

        if (trainer is null)
        {
            logger.LogWarning("Session validation failed: Trainer ID {TrainerId} does not exist.", trainerId);
            return Result.Failure("TrainerId", "Selected trainer does not exist.");
        }

        if (trainer.Speciality != category.RequiredSpeciality)
        {
            logger.LogWarning(
                "Session validation failed: Trainer ID {TrainerId} speciality {TrainerSpeciality} does not match category {CategoryId} required speciality {RequiredSpeciality}.",
                trainerId,
                trainer.Speciality,
                categoryId,
                category.RequiredSpeciality);
            return Result.Failure("TrainerId", "Selected trainer is not qualified for this category.");
        }

        var hasConflict = await unitOfWork.Sessions.ExistsAsync(
            s => (!excludeSessionId.HasValue || s.Id != excludeSessionId.Value) &&
                 s.TrainerId == trainerId &&
                 s.StartDate < endDate &&
                 startDate < s.EndDate,
            cancellationToken);

        if (hasConflict)
        {
            logger.LogWarning("Session validation failed: Trainer ID {TrainerId} has a schedule conflict.", trainerId);
            return Result.Failure("TrainerId", "This trainer already has another session scheduled during this time slot.");
        }

        return Result.Success();
    }

    private static SessionStatus GetStatus(DateTime startDate, DateTime endDate, DateTime now)
    {
        if (now < startDate)
            return SessionStatus.Upcoming;

        return now < endDate
            ? SessionStatus.Ongoing
            : SessionStatus.Completed;
    }
}
