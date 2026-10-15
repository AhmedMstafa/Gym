using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.Presentation.Controllers
{
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