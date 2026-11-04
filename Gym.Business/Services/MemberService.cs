using Gym.Business.Common;
using Gym.Business.ViewModels;
using Gym.Business.ViewModels.Members;
using Gym.DataAccess.Entities;
using Gym.DataAccess.Entities.ValueObjects;
using Gym.DataAccess.Enums;
using Gym.DataAccess.Repositories;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Gym.Business.Services
{
    internal class MemberService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IDateTimeProvider dateTimeProvider,
        IPhotoService photoService,
        ILogger<MemberService> logger) : IMemberServices
    {
        public async Task<Result<IEnumerable<MemberIndexViewModel>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Retrieving all members from database.");
            var members = await unitOfWork.Members.GetAllAsync(cancellationToken);
            var viewModels = mapper.Map<IEnumerable<MemberIndexViewModel>>(members);
            return Result.Success(viewModels);
        }

        public async Task<Result> CreateAsync(CreateMemberViewModel model, CancellationToken cancellationToken = default)
        {
            string? uploadedPhotoUrl = null;
            try
            {
                var email = model.Email.Trim().ToLower();
                var phone = model.Phone.Trim();

                if (await unitOfWork.Members.IsEmailTakenAsync(email, cancellationToken: cancellationToken))
                {
                    logger.LogWarning("Member creation failed: Email {Email} is already registered.", email);
                    return Result.Failure("Email", "This email is already registered.");
                }

                if (await unitOfWork.Members.IsPhoneTakenAsync(phone, cancellationToken: cancellationToken))
                {
                    logger.LogWarning("Member creation failed: Phone {Phone} is already in use.", phone);
                    return Result.Failure("Phone", "This phone number is already registered.");
                }

                if (!Enum.TryParse(model.Gender, true, out Gender gender))
                {
                    logger.LogWarning("Member creation failed: Invalid gender '{Gender}'.", model.Gender);
                    return Result.Failure("Gender", "Invalid gender selected.");
                }

                if (model.HealthRecord is null ||
                    !Enum.TryParse(model.HealthRecord.BloodType, true, out BloodType bloodType))
                {
                    logger.LogWarning("Member creation failed: Invalid blood type '{BloodType}'.", model.HealthRecord?.BloodType);
                    return Result.Failure("HealthRecord.BloodType", "Invalid blood type selected.");
                }

                if (model.Photo != null && model.Photo.Length > 0)
                {
                    var uploadResult = await photoService.UploadPhotoAsync(model.Photo, "members", cancellationToken);
                    if (uploadResult.IsFailure)
                    {
                        logger.LogWarning("Photo upload failed: {Error}", uploadResult.Error.Description);
                        return Result.Failure("Photo", uploadResult.Error.Description);
                    }
                    uploadedPhotoUrl = uploadResult.Value;
                }

                var member = new Member
                {
                    Name = model.Name.Trim(),
                    Email = email,
                    Phone = phone,
                    DateOfBirth = DateOnly.FromDateTime(model.DateOfBirth),
                    Gender = gender,
                    JoinDate = model.JoinDate == default ? dateTimeProvider.Now : model.JoinDate,
                    Photo = uploadedPhotoUrl,
                    Address = new Address
                    {
                        BuildingNumber = model.BuildingNumber,
                        City = model.City.Trim(),
                        Street = model.Street.Trim()
                    },
                    HealthRecord = new HealthRecord
                    {
                        Hight = model.HealthRecord.Hight,
                        Weight = model.HealthRecord.Weight,
                        Notes = model.HealthRecord.Notes,
                        BloodType = bloodType
                    }
                };

                await unitOfWork.Members.AddAsync(member, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Member {MemberName} successfully created with ID {MemberId}.", member.Name, member.Id);
                return Result.Success();
            }
            catch (DbUpdateException ex)
            {
                if (!string.IsNullOrEmpty(uploadedPhotoUrl))
                    await photoService.DeletePhotoAsync(uploadedPhotoUrl, cancellationToken);

                logger.LogWarning(ex, "Member creation failed because the email or phone is already registered.");
                return Result.Failure("DuplicateUser", "The email or phone number is already registered.");
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uploadedPhotoUrl))
                {
                    await photoService.DeletePhotoAsync(uploadedPhotoUrl, cancellationToken);
                }

                var errorMsg = ex.InnerException?.Message ?? ex.Message;
                logger.LogError(ex, "Error occurred while creating member.");
                return Result.Failure("DatabaseError", $"Could not create member: {errorMsg}");
            }
        }

        public async Task<Result<MemberDetailsViewModel>> GetDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Retrieving details for member ID {MemberId}.", id);
            var today = dateTimeProvider.DateTimeToday;

            var member = await unitOfWork.Members.GetWithActiveMembershipAsync(id: id, today: today, cancellationToken: cancellationToken);

            if (member is null)
            {
                logger.LogWarning("Member details query failed: Member with ID {MemberId} was not found.", id);
                return Result.Failure<MemberDetailsViewModel>("MemberNotFound", $"Member with ID {id} was not found.");
            }

            var viewModel = mapper.Map<MemberDetailsViewModel>(member);
            return Result.Success(viewModel);
        }

        public async Task<Result<HealthRecordViewModel>> GetHealthRecordAsync(int id, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Retrieving health record for member ID {MemberId}.", id);
            var healthRecord = await unitOfWork.HealthRecords.FindAsync(predicate: m => m.MemberId == id, cancellationToken);

            if (healthRecord is null)
            {
                logger.LogWarning("Health record query failed: Record for member ID {MemberId} was not found.", id);
                return Result.Failure<HealthRecordViewModel>("HealthRecordNotFound", $"Health record for member ID {id} was not found.");
            }

            var viewModel = mapper.Map<HealthRecordViewModel>(healthRecord);
            return Result.Success(viewModel);
        }

        public async Task<Result<EditMemberViewModel>> GetForUpdateAsync(int id, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Retrieving member for update with ID {MemberId}.", id);
            var member = await unitOfWork.Members.GetByIdAsync(id, cancellationToken);

            if (member is null)
            {
                logger.LogWarning("Member update lookup failed: Member with ID {MemberId} was not found.", id);
                return Result.Failure<EditMemberViewModel>("MemberNotFound", $"Member with ID {id} was not found.");
            }

            var viewModel = mapper.Map<EditMemberViewModel>(member);
            return Result.Success(viewModel);
        }

        public async Task<Result> UpdateAsync(int id, EditMemberViewModel model, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Updating member with ID {MemberId}.", id);
            var member = await unitOfWork.Members.GetByIdAsync(id, cancellationToken);

            if (member is null)
            {
                logger.LogWarning("Member update failed: Member with ID {MemberId} was not found.", id);
                return Result.Failure("MemberNotFound", $"Member with ID {id} was not found.");
            }

            if (member.Name != model.Name)
            {
                logger.LogWarning("Member update failed: Attempted to change immutable name for member ID {MemberId}.", id);
                return Result.Failure("Name", "Name cannot be changed.");
            }

            var normalizedEmail = model.Email.Trim().ToLowerInvariant();
            var normalizedPhone = model.Phone.Trim();

            if (await unitOfWork.Members.IsEmailTakenAsync(normalizedEmail, id, cancellationToken))
            {
                logger.LogWarning("Member update failed: Email {Email} is already taken by another member.", normalizedEmail);
                return Result.Failure(nameof(model.Email), "This email is already registered.");
            }

            if (await unitOfWork.Members.IsPhoneTakenAsync(normalizedPhone, id, cancellationToken))
            {
                logger.LogWarning("Member update failed: Phone {Phone} is already taken by another member.", normalizedPhone);
                return Result.Failure(nameof(model.Phone), "This Phone is already registered.");
            }

            string? oldPhoto = member.Photo;
            if (model.NewPhoto != null && model.NewPhoto.Length > 0)
            {
                var uploadResult = await photoService.UploadPhotoAsync(model.NewPhoto, "members", cancellationToken);
                if (uploadResult.IsFailure)
                {
                    logger.LogWarning("New photo upload failed for member {MemberId}: {Error}", id, uploadResult.Error.Description);
                    return Result.Failure("NewPhoto", uploadResult.Error.Description);
                }

                member.Photo = uploadResult.Value;
            }

            member.Email = normalizedEmail;
            member.Phone = normalizedPhone;
            member.Address = new Address
            {
                BuildingNumber = model.BuildingNumber,
                Street = model.Street.Trim(),
                City = model.City.Trim()
            };

            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Member with ID {MemberId} was updated successfully.", id);

            if (model.NewPhoto != null && !string.IsNullOrEmpty(oldPhoto))
            {
                await photoService.DeletePhotoAsync(oldPhoto, cancellationToken);
            }

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Attempting to delete member with ID {MemberId}.", id);
            var member = await unitOfWork.Members.GetByIdAsync(id: id, cancellationToken: cancellationToken);

            if (member is null)
            {
                logger.LogWarning("Member deletion failed: Member with ID {MemberId} was not found.", id);
                return Result.Failure("MemberNotFound", $"Member with ID {id} was not found.");
            }

            if (await unitOfWork.Bookings.HasUpcommingActiveSessionsAsync(id, dateTimeProvider.UtcNow, cancellationToken))
            {
                logger.LogWarning("Member deletion failed: Member with ID {MemberId} has upcoming active sessions.", id);
                return Result.Failure("ActiveSessions", "Member has upcoming active sessions and cannot be deleted.");
            }

            var photoToDelete = member.Photo;

            await unitOfWork.Members.SoftDeleteAsync(member, cancellationToken);

            var healthRecord = await unitOfWork.HealthRecords.FindAsync(h => h.MemberId == id, cancellationToken);
            if (healthRecord is not null)
            {
                await unitOfWork.HealthRecords.SoftDeleteAsync(healthRecord, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Member with ID {MemberId} was soft deleted successfully.", id);

            if (!string.IsNullOrEmpty(photoToDelete))
            {
                await photoService.DeletePhotoAsync(photoToDelete, cancellationToken);
            }

            return Result.Success();
        }
    }
}
