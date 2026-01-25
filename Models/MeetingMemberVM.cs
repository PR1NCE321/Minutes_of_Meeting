
namespace MOM.Models;

public class AttendanceVM
{
    public DateTime MeetingDate { get; set; }

    public string MeetingTypeName { get; set; }

    public string MeetingVenueName { get; set; }

    public string DepartmentName { get; set; }

    public string StaffName { get; set; }

    public string EmailAddress { get; set; }

    public bool IsPresent { get; set; }

    public string? Remarks { get; set; }
}
    