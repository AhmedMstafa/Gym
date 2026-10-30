using Gym.Business.Services;
using Gym.Business.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Controllers;

[Authorize]
public class TrainerController(ITrainerService service) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct) => View(await service.GetAllTrainersAsync(ct));

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTrainerViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await service.CreateTrainerAsync(model, ct);
        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error.Description);
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
        => (await service.GetTrainerDetailsAsync(id, ct)) is { } model ? View(model) : NotFound();

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
        => (await service.GetTrainerToUpdateAsync(id, ct)) is { } model ? View(model) : NotFound();

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
        => (await service.GetTrainerDetailsAsync(id, ct)) is not null ? View() : NotFound();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TrainerToUpdateViewModel model, CancellationToken ct)
        => !ModelState.IsValid ? View(model) : RedirectAfter(await service.UpdateTrainerDetailsAsync(id, model, ct));

    [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Delete))]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        => RedirectAfter(await service.RemoveTrainerAsync(id, ct));

    private IActionResult RedirectAfter(Gym.Business.Common.Result result)
        => result.IsSuccess ? RedirectToAction(nameof(Index)) : BadRequest(result.Error.Description);
}