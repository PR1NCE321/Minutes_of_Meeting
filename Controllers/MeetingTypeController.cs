using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using MOM.Data;
using MOM.Models;

namespace MOM.Controllers
{
    public class MeetingTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeetingTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index(string SearchText = null, int? page = null)
        {
            var searchTextParam = new SqlParameter("@SearchText", (object?)SearchText ?? DBNull.Value);
            
            var data = await _context.MeetingTypes
                .FromSqlRaw("EXEC PR_MOM_MeetingType_Search @SearchText", searchTextParam)
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
        public async Task<IActionResult> Create(MeetingTypeModel model)
        {
            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC PR_MOM_MeetingType_Insert {0}, {1}",
                    model.MeetingTypeName,
                    model.Remarks
                );
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

       
        public async Task<IActionResult> Update(int id)
        {
            var data = await _context.MeetingTypes.FindAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Update(MeetingTypeModel model)
        {
            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC PR_MOM_MeetingType_UpdateByPK {0}, {1}, {2}",
                    model.MeetingTypeID,
                    model.MeetingTypeName,
                    model.Remarks
                );
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        
        public async Task<IActionResult> Details(int id)
        {
            var meetingType = await _context.MeetingTypes
                .FirstOrDefaultAsync(m => m.MeetingTypeID == id);

            if (meetingType == null)
            {
                return NotFound();
            }

            return View(meetingType);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC PR_MOM_MeetingType_DeleteByPK {0}", id
                );
                TempData["Success"] = "Meeting Type deleted successfully.";
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                TempData["Error"] = "Cannot delete this Meeting Type because it is used in existing meetings.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the Meeting Type.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
