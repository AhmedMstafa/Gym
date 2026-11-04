using Gym.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Controllers
{
    [Authorize]
    public class PlansController : Controller
    {
        private readonly IPlanRepository _PlanRepository;

        public PlansController(IPlanRepository planRepository)
        {
            _PlanRepository = planRepository;
        }
        public async Task<IActionResult> Index()
        {



            var plans = await _PlanRepository.GetAllAsync();
            return View(plans);
        }


    public async Task<IActionResult> Details(int id)
        {
            if(id <= 0)
            {
                return NotFound();
            }

            var plan = await _PlanRepository.GetByIdAsync(id);

            if(plan is null )
            {
                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }
    }




}