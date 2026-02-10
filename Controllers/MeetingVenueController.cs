using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
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

        public async Task<IActionResult> Index(string SearchText = null, int? page = null)
        {
            var searchTextParam = new SqlParameter("@SearchText", (object?)SearchText ?? DBNull.Value);

            var data = await _context.MeetingVenues
                .FromSqlRaw("EXEC PR_MOM_MeetingVenue_Search @SearchText", searchTextParam)
                .ToListAsync();

            int pageSize = 10;
            int pageNumber = page ?? 1;
            int totalRecords = data.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var pagedList = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentSearchText = SearchText;

            return View(pagedList);
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

        public async Task<IActionResult> Details(int id)
        {
            var meetingVenue = await _context.MeetingVenues
                .FirstOrDefaultAsync(m => m.MeetingVenueID == id);

            if (meetingVenue == null)
            {
                return NotFound();
            }

            return View(meetingVenue);
        }



        public async Task<IActionResult> Update(int id)
        {
            var meetingVenue = await _context.MeetingVenues.FindAsync(id);
            if (meetingVenue == null)
            {
                return NotFound();
            }
            return View(meetingVenue);
        }

        [HttpPost]
        public async Task<IActionResult> Update(MeetingVenueModel model)
        {
            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC PR_MOM_MeetingVenue_UpdateByPK {0}, {1}",
                    model.MeetingVenueID,
                    model.MeetingVenueName
                );
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC PR_MOM_MeetingVenue_DeleteByPK {0}", id
                );
                TempData["Success"] = "Meeting Venue deleted successfully.";
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                TempData["Error"] = "Cannot delete this Venue because it is used in existing meetings.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the Venue.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
