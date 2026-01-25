using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MOM.Data;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public MeetingsController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<IActionResult> Index(
            int? MeetingTypeID = null, 
            int? DepartmentID = null, 
            int? MeetingVenueID = null, 
            DateTime? StartDate = null, 
            DateTime? EndDate = null, 
            string? SearchText = null)
        {
            // Load dropdowns for the filter section
            await LoadDropdowns();

            // Pass current filter values to View for preserving state
            ViewBag.CurrentMeetingTypeID = MeetingTypeID;
            ViewBag.CurrentDepartmentID = DepartmentID;
            ViewBag.CurrentMeetingVenueID = MeetingVenueID;
            ViewBag.CurrentStartDate = StartDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentEndDate = EndDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentSearchText = SearchText;

            List<MeetingListVM> list = new();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MOMConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("PR_MOM_Search_Meetings", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    // Add parameters
                    cmd.Parameters.AddWithValue("@MeetingTypeID", (object?)MeetingTypeID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DepartmentID", (object?)DepartmentID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MeetingVenueID", (object?)MeetingVenueID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartDate", (object?)StartDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndDate", (object?)EndDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SearchText", (object?)SearchText ?? DBNull.Value);

                    con.Open();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            list.Add(new MeetingListVM
                            {
                                MeetingID = Convert.ToInt32(dr["MeetingID"]),
                                MeetingDate = Convert.ToDateTime(dr["MeetingDate"]),
                                MeetingTypeName = dr["MeetingTypeName"]?.ToString() ?? "",
                                MeetingVenueName = dr["MeetingVenueName"]?.ToString() ?? "",
                                DepartmentName = dr["DepartmentName"]?.ToString() ?? "",
                                MeetingDescription = dr["MeetingDescription"]?.ToString() ?? "",
                                IsCancelled = Convert.ToBoolean(dr["IsCancelled"])
                            });
                        }
                    }
                }
            }

            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MeetingsModel model)
        {
            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC PR_MOM_Meetings_Insert {0},{1},{2},{3},{4},{5}",
                    model.MeetingDate,
                    model.MeetingVenueID,
                    model.MeetingTypeID,
                    model.DepartmentID,
                    model.MeetingDescription,
                    model.DocumentPath
                );
                return RedirectToAction(nameof(Index));
            }
            await LoadDropdowns();
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC PR_MOM_Meetings_DeleteByPK {0}", id
            );
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            ViewBag.MeetingTypeList = await _context.MeetingTypes
                .FromSqlRaw("EXEC PR_MOM_MeetingType_SelectAll")
                .ToListAsync();

            ViewBag.MeetingVenueList = await _context.MeetingVenues
                .FromSqlRaw("EXEC PR_MOM_MeetingVenue_SelectAll")
                .ToListAsync();

            ViewBag.DepartmentList = await _context.Departments
                .FromSqlRaw("EXEC PR_MOM_Department_SelectAll")
                .ToListAsync();
        }
    }
}
