using Gym.Business.Services;
using Gym.Business.ViewModels.Members;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Gym.Presentation.Controllers
{
    public class MembersController(IMemberServices member) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var items = await member.GetAllAsync(ct);

            return View(items);
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

            var result = await member.CreateAsync(model, ct);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(result.Error.Code, result.Error.Description);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
