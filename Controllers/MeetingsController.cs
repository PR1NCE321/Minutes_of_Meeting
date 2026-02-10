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
            string? SearchText = null,
            int? page = null)
        {
            await LoadDropdowns();

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

            int pageSize = 10;
            int pageNumber = page ?? 1;
            int totalRecords = list.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            
            var pagedList = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;

            return View(pagedList);
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
                    "EXEC PR_MOM_Meetings_Insert {0},{1},{2},{3},{4},{5},{6},{7},{8}",
                    model.MeetingDate,
                    model.MeetingVenueID,
                    model.MeetingTypeID,
                    model.DepartmentID,
                    model.MeetingDescription,
                    model.DocumentPath,
                    false, 
                    null, 
                    null  
                );
                return RedirectToAction(nameof(Index));
            }
            await LoadDropdowns();
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var meeting = await _context.Meetings
                .Include(m => m.MeetingType)
                .Include(m => m.Department)
                .Include(m => m.MeetingVenue)
                .Include(m => m.MeetingMembers)
                .ThenInclude(mm => mm.Staff)
                .FirstOrDefaultAsync(m => m.MeetingID == id);

            if (meeting == null)
            {
                return NotFound();
            }

            // Use ADO.NET to fetch staff list to avoid EF mapping issues with strict column matching
            var staffList = new List<StaffModel>();
            var connection = _context.Database.GetDbConnection();
            try
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "PR_MOM_Staff_SelectAll";
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var staff = new StaffModel
                            {
                                StaffID = Convert.ToInt32(reader["StaffID"]),
                                DepartmentID = (int)reader["DepartmentID"],
                                StaffName = reader["StaffName"].ToString(),
                                MobileNo = reader["MobileNo"] != DBNull.Value ? reader["MobileNo"].ToString() : null,
                                EmailAddress = reader["EmailAddress"] != DBNull.Value ? reader["EmailAddress"].ToString() : null,
                                Remarks = reader["Remarks"] != DBNull.Value ? reader["Remarks"].ToString() : null,
                            };
                            
                            if (reader["DepartmentName"] != DBNull.Value)
                            {
                                staff.Department = new DepartmentModel 
                                { 
                                    DepartmentID = staff.DepartmentID.Value, 
                                    DepartmentName = reader["DepartmentName"].ToString() 
                                };
                            }
                            staffList.Add(staff);
                        }
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            ViewBag.StaffList = staffList;

            return View(meeting);
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(int meetingId, int staffId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC PR_MOM_MeetingMember_Insert {0},{1},{2},{3}",
                meetingId,
                staffId,
                false, 
                "" 
            );
            return RedirectToAction(nameof(Details), new { id = meetingId });
        }

        public async Task<IActionResult> Update(int id)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }
            await LoadDropdowns();
            return View(meeting);
        }

        [HttpPost]
        public async Task<IActionResult> Update(MeetingsModel model)
        {
            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC PR_MOM_Meetings_UpdateByPK {0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                    model.MeetingID,
                    model.MeetingDate,
                    model.MeetingVenueID,
                    model.MeetingTypeID,
                    model.DepartmentID,
                    model.MeetingDescription,
                    model.DocumentPath,
                    model.IsCancelled,
                    model.CancellationDateTime,
                    model.CancellationReason
                );
                return RedirectToAction(nameof(Index));
            }
            await LoadDropdowns();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAttendance(List<MeetingMemberModel> members, int meetingId)
        {
            if (members != null && members.Any())
            {
                foreach (var member in members)
                {
                    var existingMember = await _context.MeetingMembers.FindAsync(member.MeetingMemberID);
                    if (existingMember != null)
                    {
                        existingMember.IsPresent = member.IsPresent;
                        existingMember.Modified = DateTime.Now;
                        _context.Entry(existingMember).Property(x => x.IsPresent).IsModified = true;
                        _context.Entry(existingMember).Property(x => x.Modified).IsModified = true;
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = meetingId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC PR_MOM_Meetings_DeleteByPK {0}", id
                );
                TempData["Success"] = "Meeting deleted successfully.";
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                TempData["Error"] = "Cannot delete this Meeting because it has attendance records or other dependencies.";
            }
             catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the Meeting.";
            }
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
