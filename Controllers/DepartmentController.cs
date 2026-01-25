using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MOM.Data;
using MOM.Models;

namespace MOM.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments
                .FromSqlRaw("EXEC PR_MOM_Department_SelectAll")
                .ToListAsync();

            return View(departments);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentModel model)
        {
            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC PR_MOM_Department_Insert {0}",
                    model.DepartmentName
                );
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DepartmentModel model)
        {
            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC PR_MOM_Department_UpdateByPK {0}, {1}",
                    model.DepartmentID,
                    model.DepartmentName
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
                    "EXEC PR_MOM_Department_DeleteByPK @DepartmentID",
                    new SqlParameter("@DepartmentID", id)
                );

                TempData["Success"] = "Department deleted successfully.";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    TempData["Error"] = "Cannot delete department because staff members are assigned to it.";
                }
                else
                {
                    TempData["Error"] = "An error occurred while deleting the department.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
