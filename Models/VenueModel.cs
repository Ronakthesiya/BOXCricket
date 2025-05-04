using System.ComponentModel.DataAnnotations;

namespace BOXCricket.Models
{
    public class VenueModel
    {
        [Key]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Venue name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Venue name must be between 3 and 100 characters.")]
        [Display(Name = "Venue Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; }

        [Url(ErrorMessage = "Please provide a valid URL for the location link.")]
        public string LocationLink { get; set; }

        [Required(ErrorMessage = "Price per hour is required.")]
        [Range(1, 10000, ErrorMessage = "Price per hour must be between 1 and 10,000.")]
        public int PricePerHour { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 10000, ErrorMessage = "Capacity must be a positive number.")]
        public int Capacity { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Admin ID is required.")]
        public int AdminId { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        public List<PhotoModel> Images { get; set; }

        public ReviewModel Review { get; set; }
    }
}
