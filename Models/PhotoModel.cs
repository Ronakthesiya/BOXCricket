using System.ComponentModel.DataAnnotations;

namespace BOXCricket.Models
{
    public class PhotoModel
    {
        [Key]
        public int PhotoId { get; set; }
        public int VenueId { get; set; }
        public string Photo { get; set; }
        
    }
}
