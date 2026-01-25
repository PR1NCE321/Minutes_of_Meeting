using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MOM.Data;
using MOM.Models;

namespace MOM.Controllers
{
    public class MeetingVenueController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeetingVenueController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _context.MeetingVenues
                .FromSqlRaw("EXEC PR_MOM_MeetingVenue_SelectAll")
                .ToListAsync();

            return View(data);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(MeetingVenueModel model)
        {
            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC PR_MOM_MeetingVenue_Insert {0}",
                    model.MeetingVenueName
                );
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC PR_MOM_MeetingVenue_DeleteByPK {0}", id
            );
            return RedirectToAction(nameof(Index));
        }
    }
}
