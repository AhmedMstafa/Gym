using Gym.Business.Services;
using Gym.Business.ViewModels.BookingViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym.Presentation.Controllers;

[Authorize]
public class BookingController(IBookingService service) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct) => View(await service.GetAllSessionsAsync(ct));

    [HttpGet]
    public async Task<IActionResult> Create(int id, CancellationToken ct)
    {
        await FillMembers(id, ct);
        return View(new CreateBookingViewModel { SessionId = id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookingViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) { await FillMembers(model.SessionId, ct); return View(model); }
        var result = await service.CreateNewBookingAsync(model, ct);
        if (result.IsSuccess) return RedirectToAction(nameof(Index));
        return await ModelError(result.Error.Description, model, ct);
    }

    [HttpGet]
    public async Task<IActionResult> GetMembersForUpcomingSession(int id, CancellationToken ct)
        => View("GetMembersForUpcomingSession", await service.GetMembersForUpcomingBySessionIdAsync(id, ct));

    [HttpGet]
    public async Task<IActionResult> GetMembersForOngoingSession(int id, CancellationToken ct)
        => View("GetMembersForOngoingSession", await service.GetMembersForOngoingBySessionIdAsync(id, ct));

    public Task<IActionResult> GetMembersForUpcomingSessions(int id, CancellationToken ct)
        => GetMembersForUpcomingSession(id, ct);

    public Task<IActionResult> GetMembersForOngoingSessions(int id, CancellationToken ct)
        => GetMembersForOngoingSession(id, ct);

    private async Task FillMembers(int sessionId, CancellationToken ct)
        => ViewBag.Members = new SelectList(await service.GetMembersForDropDownAsync(sessionId, ct), "Id", "Name");

    private async Task<IActionResult> ModelError(string error, CreateBookingViewModel model, CancellationToken ct)
    {
        ModelState.AddModelError(string.Empty, error);
        await FillMembers(model.SessionId, ct);
        return View("Create", model);
    }
}