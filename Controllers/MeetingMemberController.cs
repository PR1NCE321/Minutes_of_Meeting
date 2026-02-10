using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MOM.Data;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingMemberController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public MeetingMemberController(
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        
        public IActionResult Index()
        {
            List<AttendanceVM> list = new();

            using SqlConnection con =
                new SqlConnection(_configuration.GetConnectionString("MOMConnection"));

            SqlCommand cmd = new SqlCommand(
                "PR_MOM_Attendance_Join", con);

            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new AttendanceVM
                {
                    MeetingDate = Convert.ToDateTime(dr["MeetingDate"]),
                    MeetingTypeName = dr["MeetingTypeName"]?.ToString() ?? "",
                    MeetingVenueName = dr["MeetingVenueName"]?.ToString() ?? "",
                    DepartmentName = dr["DepartmentName"]?.ToString() ?? "",
                    StaffName = dr["StaffName"]?.ToString() ?? "",
                    EmailAddress = dr["EmailAddress"]?.ToString() ?? "",
                    IsPresent = Convert.ToBoolean(dr["IsPresent"]),
                    Remarks = dr["Remarks"]?.ToString() ?? ""
                });
            }

            return View(list);
        }

        
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        private async Task LoadDropdowns()
        {
            ViewBag.MeetingList = await _context.Meetings
                .FromSqlRaw("EXEC PR_MOM_Meetings_SelectAll")
                .Select(m => new { m.MeetingID, DisplayName = (m.MeetingDate.HasValue ? m.MeetingDate.Value.ToString("dd MMM yyyy") : "N/A") + " - " + m.MeetingDescription })
                .ToListAsync();

            ViewBag.StaffList = await _context.Staff
                .FromSqlRaw("EXEC PR_MOM_Staff_SelectAll")
                .ToListAsync();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeetingMemberModel model)
        {
            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC PR_MOM_MeetingMember_Insert {0},{1},{2},{3}",
                    model.MeetingID,
                    model.StaffID,
                    model.IsPresent,
                    model.Remarks
                );

                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns();
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC PR_MOM_MeetingMember_DeleteByPK {0}", id
                );
                TempData["Success"] = "Attendance record deleted successfully.";
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                 TempData["Error"] = "Cannot delete this record because it is referenced by other data.";
            }
             catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the record.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
