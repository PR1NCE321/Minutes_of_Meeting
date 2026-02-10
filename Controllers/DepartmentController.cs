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

        public async Task<IActionResult> Index(string SearchText = null, int? page = null)
        {
            var searchTextParam = new SqlParameter("@SearchText", (object?)SearchText ?? DBNull.Value);

            var departments = await _context.Departments
                .FromSqlRaw("EXEC PR_MOM_Department_Search @SearchText", searchTextParam)
                .ToListAsync();

            int pageSize = 10;
            int pageNumber = page ?? 1;
            int totalRecords = departments.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var pagedList = departments.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentSearchText = SearchText;

            return View(pagedList);
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

        public async Task<IActionResult> Update(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> Update(DepartmentModel model)
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

        public async Task<IActionResult> Details(int id)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentID == id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
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
