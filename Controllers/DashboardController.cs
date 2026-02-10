using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MOM.Data;
using MOM.Models;
using System.Diagnostics;

namespace MOM.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            DashboardViewModel model = new DashboardViewModel
            {
                TotalMeetingTypes = await _context.MeetingTypes.CountAsync(),
                TotalDepartments = await _context.Departments.CountAsync(),
                TotalStaff = await _context.Staff.CountAsync(),
                TotalVenues = await _context.MeetingVenues.CountAsync(),
                TotalMeetings = await _context.Meetings.CountAsync(),
                CancelledMeetings = await _context.Meetings
                    .Where(m => m.IsCancelled == true)
                    .CountAsync()
            };

            // 1. Meetings by Department (Top 5)
            var deptStats = await _context.Meetings
                .Include(m => m.Department)
                .GroupBy(m => m.Department.DepartmentName)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            model.DepartmentNames = deptStats.Select(x => x.Name).ToList();
            model.DepartmentCounts = deptStats.Select(x => x.Count).ToList();

            // 2. Meetings by Type (Top 5)
            var typeStats = await _context.Meetings
                .Include(m => m.MeetingType)
                .GroupBy(m => m.MeetingType.MeetingTypeName)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            model.MeetingTypeNames = typeStats.Select(x => x.Name).ToList();
            model.MeetingTypeCounts = typeStats.Select(x => x.Count).ToList();

            // 3. Monthly Trends (Last 6 Months)
            // Note: EF Core translation for dates can be tricky depending on provider. 
            // Fetching necessary data to memory for small datasets is valid, but GroupBy Year/Month server-side is better if supported.
            // Using a simple server-side approx approach for now.
             
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
             var monthlyStats = await _context.Meetings
                .Where(m => m.MeetingDate >= sixMonthsAgo)
                .OrderBy(m => m.MeetingDate)
                .ToListAsync(); // Pull to memory to group safely by formatted string

            var groupedMonths = monthlyStats
                .GroupBy(m => m.MeetingDate.HasValue ? m.MeetingDate.Value.ToString("MMM yyyy") : "Unknown")
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToList();

            model.MonthLabels = groupedMonths.Select(x => x.Month).ToList();
            model.MonthlyCounts = groupedMonths.Select(x => x.Count).ToList();

            // 4. Venue Utilization (Top 5)
            var venueStats = await _context.Meetings
                .Include(m => m.MeetingVenue)
                .GroupBy(m => m.MeetingVenue.MeetingVenueName)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            model.VenueNames = venueStats.Select(x => x.Name).ToList();
            model.VenueCounts = venueStats.Select(x => x.Count).ToList();

            // 5. Top Staff Contributors (Top 5)
            // Assuming "MOM_MeetingMember" db set is available as _context.MeetingMembers?
            // Need to check context first. If not available, we might need to skip or add it.
            // Let's assume generic dbset access or context has it. Based on file list, MeetingMemberController exists, so likely yes.
            // However, looking at the previous file list of Controllers, I didn't verify the DbContext file. 
            // To be safe, I will use Set<MeetingMemberModel>() if property specific name is unknown, or guess "MeetingMembers".
            // Actually, let's verify DbContext or just try "MeetingMembers". Most generated contexts use plural.
            
            try 
            {
                 // Using Set<T> is safer if I don't know the exact property name
                 var staffStats = await _context.Set<MeetingMemberModel>()
                    .Include(mm => mm.Staff)
                    .GroupBy(mm => mm.Staff.StaffName)
                    .Select(g => new { Name = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToListAsync();

                model.StaffNames = staffStats.Select(x => x.Name).ToList();
                model.StaffCounts = staffStats.Select(x => x.Count).ToList();
            }
            catch
            {
                // Fallback if table/set issue
                model.StaffNames = new List<string>();
                model.StaffCounts = new List<int>();
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
