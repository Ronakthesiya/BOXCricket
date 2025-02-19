using System.ComponentModel.DataAnnotations;

namespace BOXCricket.Models
{
    public class ReviewModel
    {
        [Key]
        public int ReviewId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]

        public int VenueId { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }
}
