namespace MOM.Models
{
    public class DashboardViewModel
    {
        public int TotalMeetingTypes { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalStaff { get; set; }
        public int TotalVenues { get; set; }
        public int TotalMeetings { get; set; }
        public int CancelledMeetings { get; set; }

        // Chart Data
        public List<string> DepartmentNames { get; set; } = new List<string>();
        public List<int> DepartmentCounts { get; set; } = new List<int>();

        public List<string> MeetingTypeNames { get; set; } = new List<string>();
        public List<int> MeetingTypeCounts { get; set; } = new List<int>();

        // For Monthly Trends (Last 6 months)
        public List<string> MonthLabels { get; set; } = new List<string>();
        public List<int> MonthlyCounts { get; set; } = new List<int>();

        // Venue Utilization
        public List<string> VenueNames { get; set; } = new List<string>();
        public List<int> VenueCounts { get; set; } = new List<int>();

        // Top Staff Contributors
        public List<string> StaffNames { get; set; } = new List<string>();
        public List<int> StaffCounts { get; set; } = new List<int>();
    }
}
