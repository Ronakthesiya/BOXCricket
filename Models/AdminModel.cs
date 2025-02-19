using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOXCricket.Models
{
    public class AdminModel
    {
        [Key]
        public int AdminId { get; set; }
        [Required]

        public string Name { get; set; }
        [Required]

        public string Email { get; set; }


        [Required]
        public string PasswordHash { get; set; }

        public string? id { get; set; }

        [Required]
        public string Role { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        [NotMapped]
        public DateTime CreatedAt { get; set; }

    }
}
