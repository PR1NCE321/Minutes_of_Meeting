using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using Microsoft.EntityFrameworkCore;
using MOM.Data;

namespace MOM.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var staffList = await _context.Staff
                .Include(s => s.Department)
                .Select(s => new StaffListVM
                {
                    StaffID = s.StaffID,
                    StaffName = s.StaffName,
                    DepartmentName = s.Department.DepartmentName,
                    EmailAddress = s.EmailAddress,
                    MobileNo = s.MobileNo
                })
                .ToListAsync();

            return View(staffList);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StaffModel model)
        {
            if (ModelState.IsValid)
            {
                // Easy Way: Add and Save
                _context.Staff.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await LoadDropdowns();
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            await LoadDropdowns();
            return View("Create", staff); // Reuse Create view for Edit if simple
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StaffModel model)
        {
             if (ModelState.IsValid)
            {
                _context.Staff.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await LoadDropdowns();
            return View("Create", model);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff != null)
            {
                _context.Staff.Remove(staff);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var staff = await _context.Staff
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.StaffID == id);

            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        private async Task LoadDropdowns()
        {
            ViewBag.DepartmentList = await _context.Departments.ToListAsync();
        }
    }
}
