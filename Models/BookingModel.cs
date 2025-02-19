using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOXCricket.Models
{
    public class BookingModel
    {
        [Key]
        public int BookingId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]

        public int VenueId { get; set; }
        [Required]

        public DateTime BookingDate { get; set; }
        [Required]

        public TimeSpan StartTime { get; set; }
        [Required]

        public TimeSpan EndTime { get; set; }
        [Required]

        public decimal TotalAmount { get; set; }
        [Required]

        public string Status { get; set; }

        [Display(Name = "Booked At")]
        public DateTime CreatedAt { get; set; }
        public VenueModel Venue { get; set; }

        public UserModel User { get; set; }
        
    }
}
