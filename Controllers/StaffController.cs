using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using Microsoft.EntityFrameworkCore;
using MOM.Data;
using Microsoft.Data.SqlClient;

namespace MOM.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string SearchText = null, int? page = null)
        {
            var staffList = new List<StaffListVM>();
            var connection = _context.Database.GetDbConnection();
            
            try
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "PR_MOM_Staff_SelectAll";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            staffList.Add(new StaffListVM
                            {
                                StaffID = Convert.ToInt32(reader["StaffID"]),
                                StaffName = reader["StaffName"].ToString(),
                                DepartmentName = reader["DepartmentName"] != DBNull.Value ? reader["DepartmentName"].ToString() : "N/A",
                                EmailAddress = reader["EmailAddress"] != DBNull.Value ? reader["EmailAddress"].ToString() : "",
                                MobileNo = reader["MobileNo"] != DBNull.Value ? reader["MobileNo"].ToString() : ""
                            });
                        }
                    }
                }
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    await connection.CloseAsync();
            }

            // In-Memory Search
            if (!string.IsNullOrEmpty(SearchText))
            {
                SearchText = SearchText.ToLower();
                staffList = staffList.Where(s => 
                    (s.StaffName?.ToLower().Contains(SearchText) ?? false) ||
                    (s.DepartmentName?.ToLower().Contains(SearchText) ?? false) ||
                    (s.EmailAddress?.ToLower().Contains(SearchText) ?? false) ||
                    (s.MobileNo?.ToLower().Contains(SearchText) ?? false)
                ).ToList();
            }

            // Client-side pagination
            int pageSize = 10;
            int pageNumber = page ?? 1;
            int totalRecords = staffList.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var pagedStaff = staffList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Fetch statistics for the paged staff members
            if (pagedStaff.Any())
            {
                var staffIds = pagedStaff.Select(s => s.StaffID).ToList();
                var stats = await _context.MeetingMembers
                    .Where(mm => staffIds.Contains(mm.StaffID))
                    .GroupBy(mm => mm.StaffID)
                    .Select(g => new
                    {
                        StaffID = g.Key,
                        Total = g.Count(),
                        Present = g.Count(x => x.IsPresent)
                    })
                    .ToListAsync();

                foreach (var s in pagedStaff)
                {
                    var stat = stats.FirstOrDefault(x => x.StaffID == s.StaffID);
                    int total = stat?.Total ?? 0;
                    int present = stat?.Present ?? 0;
                    s.TotalMeetings = total;
                    s.AttendanceRate = total > 0 ? (double)present / total * 100 : 0;
                }
            }

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentSearchText = SearchText;

            return View(pagedStaff);
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
                
                _context.Staff.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await LoadDropdowns();
            return View(model);
        }

        public async Task<IActionResult> Update(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            await LoadDropdowns();
            return View(staff); 
        }

        [HttpPost]
        public async Task<IActionResult> Update(StaffModel model)
        {
             if (ModelState.IsValid)
            {
                _context.Staff.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await LoadDropdowns();
            return View(model);
        }


        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var staff = await _context.Staff.FindAsync(id);
                if (staff != null)
                {
                    _context.Staff.Remove(staff);
                    await _context.SaveChangesAsync();
                }
                TempData["Success"] = "Staff Member deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                {
                    TempData["Error"] = "Cannot delete this Staff Member because they are linked to existing meetings.";
                }
                else
                {
                    TempData["Error"] = "An error occurred while deleting the Staff Member.";
                }
            }
             catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the Staff Member.";
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

            // Fetch meetings for this staff member
            var staffMeetings = await _context.MeetingMembers
                .Include(mm => mm.Meeting)
                    .ThenInclude(m => m.MeetingType)
                .Include(mm => mm.Meeting)
                    .ThenInclude(m => m.MeetingVenue)
                .Where(mm => mm.StaffID == id)
                .OrderByDescending(mm => mm.Meeting.MeetingDate)
                .ToListAsync();

            ViewBag.StaffMeetings = staffMeetings;

            return View(staff);
        }

        private async Task LoadDropdowns()
        {
            ViewBag.DepartmentList = await _context.Departments.ToListAsync();
        }
    }
}
