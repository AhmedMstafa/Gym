using Gym.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.Controllers
{
    public class PlansController : Controller
    {
        public GymDbContext Context = new GymDbContext();
        public async Task<IActionResult> Index()
        {



            var plans = await Context.Plans.ToListAsync();
            return View(plans);
        }


    public async Task<IActionResult> Details(int id)
        {
            if(id <= 0)
            {
                return NotFound();
            }

            var plan = await Context.Plans.FirstOrDefaultAsync(p => p.Id == id);

            if(plan is null )
            {
                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }
    }




}