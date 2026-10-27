using Gym.Business.Services;
using Gym.Business.ViewModels.Members;
using Gym.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Controllers
{
    public class MembersController(IMemberServices members) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken ct)
            => this.ViewIndexOrEmpty(await members.GetAllAsync(ct), "Could not load members.");

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await members.CreateAsync(model, ct);
            if (!result.IsSuccess)
            {
                return this.ViewFormOnFailure(result, model, "Cannot Create a Member");
            }

            TempData["Success"] = "Member Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            return this.ViewOrNotFound(await members.GetDetailsAsync(id, ct));
        }

        [HttpGet]
        public async Task<IActionResult> HealthRecord(int id, CancellationToken ct)
        {
            return this.ViewOrNotFound(await members.GetHealthRecordAsync(id, ct));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            return this.ViewOrNotFound(await members.GetForUpdateAsync(id, ct));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, EditMemberViewModel editMemberViewModel, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(editMemberViewModel);

            var result = await members.UpdateAsync(id, editMemberViewModel, ct);

            if (!result.IsSuccess)
            {
                return this.ViewFormOnFailure(result, editMemberViewModel, "Cannot Update a Member");
            }

            TempData["Success"] = "Member Updated Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await members.GetDetailsAsync(id, ct);

            if (result.IsFailure) return NotFound();

            ViewBag.id = result.Value.Id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await members.DeleteAsync(id, ct);

            if (!result.IsSuccess)
            {
                TempData["Error"] = "Cannot Delete a Member";
                ViewBag.id = id;

                return View();
            }
            TempData["Success"] = "Member Deleted Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
