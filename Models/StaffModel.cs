using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOM.Models;

[Table("MOM_Staff")]

public class StaffModel : IValidatableObject
{
    [Key]
    public int StaffID { get; set; }

    [Display(Name = "Department")]
    [Required(ErrorMessage = "Please select a department")]
    public int? DepartmentID { get; set; }

    [Display(Name = "Staff Name")]
    [Required(ErrorMessage = "Please enter staff name")]
    [StringLength(50)]
    public string StaffName { get; set; }

    [Display(Name = "Mobile Number")]
    [Required(ErrorMessage = "Please enter mobile number")]
    [StringLength(20)]
    public string MobileNo { get; set; }

    [Display(Name = "Email Address")]
    [Required(ErrorMessage = "Please enter email address")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(50)]
    public string EmailAddress { get; set; }

    [StringLength(250)]
    public string? Remarks { get; set; }

    
    [ForeignKey("DepartmentID")]
    public virtual DepartmentModel? Department { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;

    public DateTime Modified { get; set; } = DateTime.Now;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(MobileNo) && !MobileNo.All(char.IsDigit))
        {
            yield return new ValidationResult("Mobile number should contain only digits.", new[] { nameof(MobileNo) });
        }
    }
}