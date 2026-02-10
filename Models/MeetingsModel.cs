using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOM.Models;

[Table("MOM_Meetings")]

public class MeetingsModel : IValidatableObject
{
    [Key]
    public int MeetingID { get; set; }

    [Display(Name = "Meeting Date")]
    [Required(ErrorMessage = "Please select a date and time")]
    public DateTime? MeetingDate { get; set; }

    [Display(Name = "Meeting Venue")]
    [Required(ErrorMessage = "Please select a venue")]
    public int? MeetingVenueID { get; set; }

    [Display(Name = "Meeting Type")]
    [Required(ErrorMessage = "Please select a meeting type")]
    public int? MeetingTypeID { get; set; }

    [Display(Name = "Department")]
    [Required(ErrorMessage = "Please select a department")]
    public int? DepartmentID { get; set; }

    [ForeignKey("MeetingVenueID")]
    public virtual MeetingVenueModel? MeetingVenue { get; set; }

    [ForeignKey("MeetingTypeID")]
    public virtual MeetingTypeModel? MeetingType { get; set; }

    [ForeignKey("DepartmentID")]
    public virtual DepartmentModel? Department { get; set; }

    public virtual ICollection<MeetingMemberModel>? MeetingMembers { get; set; }

    [StringLength(250)]
    public string? MeetingDescription { get; set; }

    [StringLength(250)]
    public string? DocumentPath { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;

    public DateTime Modified { get; set; } = DateTime.Now;

    public bool? IsCancelled { get; set; }

    



    public DateTime? CancellationDateTime { get; set; }

    [StringLength(250)]
    public string? CancellationReason { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MeetingDate < DateTime.Now)
        {
            yield return new ValidationResult("Meeting date cannot be in the past.", new[] { nameof(MeetingDate) });
        }
    }
}