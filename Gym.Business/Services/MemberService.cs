using Gym.Business.Common;
using Gym.Business.ViewModels;
using Gym.Business.ViewModels.Members;
using Gym.DataAccess.Entities;
using Gym.DataAccess.Entities.ValueObjects;
using Gym.DataAccess.Enums;
using Gym.DataAccess.Repositories;

namespace Gym.Business.Services
{
    internal class MemberService(IMemberRepository memberRepo, IRepository<HealthRecord> healthReordRepo) : IMemberServices
    {
        public async Task<IEnumerable<MemberIndexViewModel>> GetAllAsync(CancellationToken cancellation = default)
        {
            var members = await memberRepo.GetAllAsync(cancellation);

            return members.Select(member => new MemberIndexViewModel
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Photo = member.Photo,
                JoinDate = member.JoinDate,
                Gender = member.Gender.ToString()
            });
        }

        public async Task<Result> CreateAsync(CreateMemberViewModel model, CancellationToken cancellationToken = default)
        {
            var email = model.Email.Trim().ToLower();
            var phone = model.Phone.Trim();
            var name = model.Name.Trim();
            var dateofbirth = model.DateOfBirth;
            var joindate = model.JoinDate;

            if (await memberRepo.IsEmailTakenAsync(email, cancellationToken: cancellationToken))
            {
                return Result.Failure("Email", "This email is already registered.");
            }


            if (await memberRepo.IsPhoneTakenAsync(phone,  cancellationToken: cancellationToken))
            {
                return Result.Failure("Phone", "This phone number is already registered.");
            }



            if (!Enum.TryParse(model.Gender, true, out Gender gender))
            {
                return Result.Failure("Gender", "Invalid gender selected.");
            }

            if (!Enum.TryParse(model.HealthRecord.BloodType, true, out BloodType bloodtype))
            {
                return Result.Failure("HealthRecord.BloodType", "Invalid blood type selected.");
            }

            var member = new Member
            {
                Name = name,
                Email = email,
                Phone = phone,
                JoinDate = joindate,
                DateOfBirth = DateOnly.FromDateTime(dateofbirth),
                Gender = gender,
                Address = new Address
                {
                    BuildingNumber = model.BuildingNumber,
                    City = model.City.Trim(),
                    Street = model.Street.Trim()
                },
                HealthRecord = new HealthRecord
                {
                    BloodType = bloodtype,
                    Hight = model.HealthRecord.Hight,
                    Weight = model.HealthRecord.Weight,
                    Notes = model.HealthRecord.Notes
                }
            };

            try
            {
                await memberRepo.AddAsync(member, cancellationToken);
                await memberRepo.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException?.Message ?? ex.Message;
                return Result.Failure(string.Empty, $"Database Error: {errorMsg}");
            }
        }

        public async Task<MemberDetailsViewModel?> GetDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            var member = await memberRepo.GetWithMembershipAsync(id: id, cancellationToken: cancellationToken);


            if (member is null) return null;

            var today = DateTime.Today;

            var activeMemberShip = member.MemberShips.FirstOrDefault(m => m.EndDate >= today);

            return new MemberDetailsViewModel
            {
                Id = member.Id,
                Name = member.Name,
                Photo = member.Photo,
                Email = member.Email,
                Phone = member.Phone,
                Gender = member.Gender.ToString(),
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City}",
                PlanName = activeMemberShip?.Plan.Name ?? "No Active Plan",
                MemberShipStartDate = activeMemberShip?.StartDate.ToShortDateString() ?? "-",
                MemberShipEndDate = activeMemberShip?.EndDate.ToShortDateString() ?? "-"
            };
        }

        public async Task<HealthRecordViewModel?> GetHealthRecordAsync(int id, CancellationToken cancellationToken)
        {
            var healthRecord = await healthReordRepo.FindAsync(predicate: m => m.MemberId == id, cancellationToken);

            if (healthRecord is null) return null;


            return new HealthRecordViewModel
            {
                Hight = healthRecord.Hight,
                Weight = healthRecord.Weight,
                BloodType = healthRecord.BloodType.ToString(),
                Notes = healthRecord.Notes
            };
        }

        public async Task<EditMemberViewModel?> GetForUpdateAsync(int id, CancellationToken cancellationToken)
        {
            var member = await memberRepo.GetByIdAsync(id, cancellationToken);

            if (member is null) return null;

            return new EditMemberViewModel
            {
                Name = member.Name,
                Photo = member.Photo,
                Phone = member.Phone,
                Email = member.Email,
                BuildingNumber = member.Address.BuildingNumber,
                City = member.Address.City,
                Street = member.Address.Street
            };
        }

        public async Task<Result> UpdateAsync(int id,EditMemberViewModel model, CancellationToken cancellationToken = default)
        {
            var member = await memberRepo.GetByIdAsync(id, cancellationToken);

            if (member is null) return Result.Failure("Member Not Found", nameof(id));

            var normalizationEmail = model.Email.Trim().ToLowerInvariant();
            var normalizationPhone = model.Phone.Trim();


            if (await memberRepo.IsEmailTakenAsync(normalizationEmail, id, cancellationToken))
            {
                return Result.Failure(nameof(model.Email), "This email is already registered.");
            }
            ;


            if (await memberRepo.IsPhoneTakenAsync(normalizationPhone, id, cancellationToken))
            {
                return Result.Failure(nameof(model.Phone), "This Phone is already registered.");
            }
            ;

            member.Email = normalizationEmail;
            member.Phone = normalizationPhone;
            member.Address = new Address
            {
                BuildingNumber = model.BuildingNumber,
                Street = model.Street,
                City = model.City
            };

            await memberRepo.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var member = await memberRepo.GetByIdAsync(id: id, cancellationToken: cancellationToken);
             
            if (member is null)
                return Result.Failure(nameof(id), "Member Not Found");

            if (await memberRepo.HasUpcommingActiveSessionsAsync(id, cancellationToken))
                return Result.Failure(nameof(id), "Member Has Active Sessions");

            await memberRepo.SoftDeleteAsync(member, cancellationToken);

            await memberRepo.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
