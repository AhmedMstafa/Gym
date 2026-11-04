using Gym.Business.Services;
using Gym.Business.ViewModels.MembershipViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym.Presentation.Controllers;

[Authorize]
public class MembershipController(IMembershipService service) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct) => View(await service.GetAllMembershipsAsync(ct));

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await FillLists(ct);
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMemberShipViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) { await FillLists(ct); return View(model); }
        var result = await service.CreateMembershipAsync(model, ct);
        if (result.IsSuccess) return RedirectToAction(nameof(Index));
        return await ModelError(result.Error.Description, model, ct);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var result = await service.DeleteMembershipAsync(id, ct);
        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.Error.Description;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task FillLists(CancellationToken ct)
    {
        ViewBag.Members = new SelectList(await service.GetMembersForDropDownAsync(ct), "Id", "Name");
        ViewBag.Plans = new SelectList(await service.GetPlansForDropDownAsync(ct), "Id", "Name");
    }

    private async Task<IActionResult> ModelError(string error, CreateMemberShipViewModel model, CancellationToken ct)
    {
        ModelState.AddModelError(string.Empty, error);
        await FillLists(ct);
        return View("Create", model);
    }
}