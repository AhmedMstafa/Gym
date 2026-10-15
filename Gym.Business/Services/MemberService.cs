using Gym.Business.Common;
using Gym.Business.ViewModels.Members;
using Gym.DataAccess.Entities;
using Gym.DataAccess.Entities.ValueObjects;
using Gym.DataAccess.Enums;
using Gym.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gym.Business.Services
{
    internal class MemberService(IMemberRepository memberRepo) : IMemberServices
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

            if (await memberRepo.ExistsAsync(m => m.Email == email, cancellationToken))
            {
                return Result.Failure("Email", "This email is already registered.");
            }

            if (await memberRepo.ExistsAsync(m => m.Phone == phone, cancellationToken))
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
    }
}
