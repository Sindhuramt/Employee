using System.ComponentModel.DataAnnotations;

namespace Employee.Models
{
    public class EmployeeModel
    {
        public int ID { get; set; }

        [Required]
        
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfJoining { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Allow Picture to be nullable
        public string? Picture { get; set; }
    }
}
