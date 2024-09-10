using System.ComponentModel.DataAnnotations;

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
        public decimal HoursWorked { get; set; }
        [Required]
        public decimal RatePerHour { get; set; }
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

