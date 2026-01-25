using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // LIST
        public async Task<IActionResult> Index()
        {
            var data = await _context.MeetingTypes
                .FromSqlRaw("EXEC PR_MOM_MeetingType_SelectAll")
                .ToListAsync();

            return View(data);
        }

        // ADD
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

       //edit
        public async Task<IActionResult> Update(int id)
        {
            var data = await _context.MeetingTypes.FindAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MeetingTypeModel model)
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

        // DELETE
        public async Task<IActionResult> Delete(int id)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC PR_MOM_MeetingType_DeleteByPK {0}", id
            );
            return RedirectToAction(nameof(Index));
        }
    }
}
