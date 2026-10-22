using Gym.Business.Services;
using Gym.Business.ViewModels.Members;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Controllers
{
    public class MembersController(IMemberServices members) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var result = await members.GetAllAsync(ct);

            return View(result.Value);
        }

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
                ModelState.AddModelError(result.Error.Code, result.Error.Description);
                TempData["Error"] = "Cannot Create a Member";

                return View(model);
            }
            TempData["Success"] = "Member Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await members.GetDetailsAsync(id, ct);

            if (result.IsFailure) return NotFound();

            return View(result.Value); 
        }

        [HttpGet]
        public async Task<IActionResult> HealthRecord(int id, CancellationToken ct)
        {
            var result = await members.GetHealthRecordAsync(id, ct);

            if (result.IsFailure) return NotFound();

            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await members.GetForUpdateAsync(id, ct);

            if (result.IsFailure) return NotFound();

            return View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, EditMemberViewModel editMemberViewModel, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(editMemberViewModel);

            var result = await members.UpdateAsync(id, editMemberViewModel, ct);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(result.Error.Code, result.Error.Description);
                TempData["Error"] = "Cannot Update a Member";

                return View(editMemberViewModel);
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
