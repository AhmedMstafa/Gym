using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.Presentation.Controllers
{
    public class PlansController : Controller
    {
        public IPlanRepository PlanRepository = new PlanRepository();
        public async Task<IActionResult> Index()
        {



            var plans = await PlanRepository.GetAllAsync();
            return View(plans);
        }


    public async Task<IActionResult> Details(int id)
        {
            if(id <= 0)
            {
                return NotFound();
            }

            var plan = await PlanRepository.GetPlanByIdAsync(id);

            if(plan is null )
            {
                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }
    }




}