using Gym.Business.Common;
using Gym.Business.ViewModels.TrainerViewModels;
using Gym.DataAccess.Entities;
using Gym.DataAccess.Entities.ValueObjects;
using Gym.DataAccess.Repositories;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Gym.Business.Services
{
    internal sealed class TrainerService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IDateTimeProvider dateTimeProvider,
        ILogger<TrainerService> logger) : ITrainerService
    {
        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
            => (await unitOfWork.Trainers.GetAllAsync(ct)).Select(ToViewModel).ToList();

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.Trainers.GetByIdAsync(trainerId, ct, false);
            return trainer is null ? null : ToViewModel(trainer);
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.Trainers.GetByIdAsync(trainerId, ct, false);
            return trainer is null ? null : new TrainerToUpdateViewModel
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Specialties = trainer.Speciality,
                BuildingNumber = trainer.Address?.BuildingNumber ?? 0,
                City = trainer.Address?.City ?? string.Empty,
                Street = trainer.Address?.Street ?? string.Empty
            };
        }

        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var email = model.Email.Trim().ToLowerInvariant();
            var phone = model.Phone.Trim();
            if (await unitOfWork.Trainers.IsEmailTakenAsync(email, ct))
                return Result.Failure("Email", "This email is already registered.");
            if (await unitOfWork.Trainers.IsPhoneTakenAsync(phone, ct))
                return Result.Failure("Phone", "This phone number is already registered.");

            var trainer = new Trainer
            {
                Name = model.Name,
                Email = email,
                Phone = phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Speciality = model.Specialties,
                Address = new Address { BuildingNumber = model.BuildingNumber, City = model.City, Street = model.Street }
            };

            try
            {
                await unitOfWork.Trainers.AddAsync(trainer, ct);
                await unitOfWork.SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                logger.LogWarning("Trainer creation failed because the email or phone is already registered.");
                return Result.Failure("DuplicateUser", "The email or phone number is already registered.");
            }
            logger.LogInformation("Trainer {TrainerName} created with ID {TrainerId}.", trainer.Name, trainer.Id);
            return Result.Success();
        }

        public async Task<Result> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.Trainers.GetByIdAsync(trainerId, ct);
            if (trainer is null) return Result.Failure("TrainerNotFound", "Trainer was not found.");

            trainer.Name = model.Name;
            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Speciality = model.Specialties;
            trainer.Address = new Address { BuildingNumber = model.BuildingNumber, City = model.City, Street = model.Street };
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }

        public async Task<Result> RemoveTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.Trainers.GetByIdAsync(trainerId, ct);
            if (trainer is null) return Result.Failure("TrainerNotFound", "Trainer was not found.");

            await unitOfWork.Trainers.SoftDeleteAsync(trainer, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }

        private static TrainerViewModel ToViewModel(Trainer trainer) => new()
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Email = trainer.Email,
            Phone = trainer.Phone,
            DateOfBirth = trainer.DateOfBirth.ToString("d"),
            Gender = trainer.Gender.ToString(),
            Address = trainer.Address is null ? string.Empty : $"{trainer.Address.BuildingNumber}, {trainer.Address.Street}, {trainer.Address.City}",
            Specialties = trainer.Speciality.ToString()
        };
    }
}
