using Gym.Business.Common;
using Gym.Business.ViewModels.BookingViewModels;
using Gym.Business.ViewModels.MembershipViewModels;
using Gym.Business.ViewModels.Sessions;
using Gym.DataAccess.Entities;
using Gym.DataAccess.Repositories;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace Gym.Business.Services
{
    internal sealed class BookingService(
        IUnitOfWork unitOfWork,
        ISessionService sessionService,
        IMapper mapper,
        IDateTimeProvider dateTimeProvider,
        ILogger<BookingService> logger) : IBookingService
    {
        public Task<IEnumerable<SessionIndexViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
            => GetSessionsAsync(ct);

        private async Task<IEnumerable<SessionIndexViewModel>> GetSessionsAsync(CancellationToken ct)
            => await sessionService.GetIndexItemsAsync(ct);

        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForUpcomingBySessionIdAsync(int sessionId, CancellationToken ct = default)
            => await GetMembersForSessionAsync(sessionId, ct);

        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForOngoingBySessionIdAsync(int sessionId, CancellationToken ct = default)
            => await GetMembersForSessionAsync(sessionId, ct);

        public async Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropDownAsync(int sessionId, CancellationToken ct = default)
        {
            var bookedMemberIds = (await unitOfWork.Bookings.GetBySessionIdAsync(sessionId, ct))
                .Select(x => x.MemberId).ToHashSet();
            return (await unitOfWork.Members.GetAllAsync(ct))
                .Where(x => !bookedMemberIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .Select(x => new MemberSelectListViewModel { Id = x.Id, Name = x.Name });
        }

        public async Task<Result> CreateNewBookingAsync(CreateBookingViewModel model, CancellationToken ct = default)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(model.SessionId, ct, false);
            var memberExists = await unitOfWork.Members.GetByIdAsync(model.MemberId, ct, false) is not null;
            if (session is null || !memberExists)
                return Result.Failure("InvalidBooking", "The selected member or session was not found.");

            var alreadyBooked = await unitOfWork.Bookings.GetByMemberAndSessionAsync(model.MemberId, model.SessionId, ct) is not null;
            if (alreadyBooked)
                return Result.Failure("BookingExists", "This member is already booked for the selected session.");

            var bookedCount = await unitOfWork.Bookings.CountBySessionIdAsync(model.SessionId, ct);
            if (bookedCount >= session.Capacity)
                return Result.Failure("SessionFull", "The selected session is full.");

            await unitOfWork.Bookings.AddAsync(new Booking
            {
                MemberId = model.MemberId,
                SessionId = model.SessionId,
                Date = dateTimeProvider.UtcNow
            }, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }

        public async Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await unitOfWork.Bookings.GetByMemberAndSessionAsync(memberId, sessionId, ct);
            if (booking is null) return Result.Failure("BookingNotFound", "Booking was not found.");

            await unitOfWork.Bookings.SoftDeleteAsync(booking, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }

        public async Task<Result> MarkAttendedAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await unitOfWork.Bookings.GetByMemberAndSessionAsync(memberId, sessionId, ct);
            if (booking is null) return Result.Failure("BookingNotFound", "Booking was not found.");

            booking.IsAttended = true;
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }

        private async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForSessionAsync(int sessionId, CancellationToken ct)
            => (await unitOfWork.Bookings.GetBySessionIdAsync(sessionId, ct))
                .Select(x => new MemberForSessionViewModel
                {
                    MemberId = x.MemberId,
                    SessionId = x.SessionId,
                    MemberName = x.Member.Name,
                    BookingDate = x.Date.ToString("g"),
                    IsAttended = x.IsAttended
                }).ToList();
    }
}
