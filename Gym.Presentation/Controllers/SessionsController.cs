using Gym.Business.Services;
using Gym.Business.ViewModels.Sessions;
using Gym.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gym.Presentation.Controllers
{
    [Authorize]
    public class SessionsController(ISessionService sessionService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            return View(await sessionService.GetIndexItemsAsync(ct));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            return this.ViewOrNotFound(await sessionService.GetDetailsAsync(id, ct));
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropdownsAsync(ct);
            var model = new CreateSessionViewModel
            {
                StartDate = DateTime.Today.AddDays(1).AddHours(9),
                EndDate = DateTime.Today.AddDays(1).AddHours(10),
                Capacity = 10
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(ct, model.CategoryId, model.TrainerId);
                return View(model);
            }

            var result = await sessionService.CreateAsync(model, ct);
            if (!result.IsSuccess)
            {
                await PopulateDropdownsAsync(ct, model.CategoryId, model.TrainerId);
                return this.ViewFormOnFailure(result, model, "Cannot Create Session");
            }

            TempData["Success"] = "Session Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await sessionService.GetForUpdateAsync(id, ct);
            if (result.IsFailure)
            {
                if (result.Error.Code is "SessionNotFound")
                    return NotFound();

                TempData["Error"] = result.Error.Description;
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdownsAsync(ct, result.Value.CategoryId, result.Value.TrainerId);
            return View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, EditSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(ct, model.CategoryId, model.TrainerId);
                return View(model);
            }

            var result = await sessionService.UpdateAsync(id, model, ct);
            if (!result.IsSuccess)
            {
                await PopulateDropdownsAsync(ct, model.CategoryId, model.TrainerId);
                return this.ViewFormOnFailure(result, model, "Cannot Update Session");
            }

            TempData["Success"] = "Session Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await sessionService.GetDetailsAsync(id, ct);

            if (result.IsFailure)
                return NotFound();

            ViewBag.id = result.Value.Id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await sessionService.DeleteAsync(id, ct);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Error.Description;
                ViewBag.id = id;
                return View();
            }

            TempData["Success"] = "Session Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> TrainersForCategory(int categoryId, CancellationToken ct)
        {
            var trainers = await sessionService.GetTrainersByCategoryAsync(categoryId, ct);
            return Json(trainers.Select(t => new { id = t.Id, name = t.Name }));
        }

        private async Task PopulateDropdownsAsync(
            CancellationToken ct,
            int? categoryId = null,
            int? selectedTrainerId = null)
        {
            var categories = await sessionService.GetCategoriesAsync(ct);
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);

            if (categoryId is > 0)
            {
                var trainers = await sessionService.GetTrainersByCategoryAsync(categoryId.Value, ct);
                ViewBag.Trainers = new SelectList(trainers, "Id", "Name", selectedTrainerId);
            }
            else
            {
                ViewBag.Trainers = new SelectList(Array.Empty<SelectListItem>(), "Value", "Text");
            }
        }
    }
}
