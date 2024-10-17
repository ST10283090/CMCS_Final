using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS_Final.Models
{
    public class Claims
    {
        public int Id { get; set; }

        [Required]
        public string Lecturer_ID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime ClaimsPeriodStart { get; set; }

        [Required]
        public DateTime ClaimsPeriodEnd { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]  
        public decimal HoursWorked { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]  
        public decimal RatePerHour { get; set; }

        [Column(TypeName = "decimal(18, 2)")]  
        public decimal TotalAmount { get; set; }

        [Required]
        public string DescriptionOfWork { get; set; }

        public string Status { get; set; }

        [Required]
        [Display(Name = "Supporting Document")]
        public string DocumentFileName { get; set; }

        public string DocumentFilePath { get; set; }
    }
}


