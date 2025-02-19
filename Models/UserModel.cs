using System.ComponentModel.DataAnnotations;

namespace BOXCricket.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        [Required]

        [Display(Name = "User")]
        public string Name { get; set; }
        [Required]

        public string Email { get; set; }
        [Required]

        public string PasswordHash { get; set; }

        public string Role { get; set; }
        [Required]

        public string PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? id { get; set; }
    }
}
