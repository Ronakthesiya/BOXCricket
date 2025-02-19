using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BOXCricket.Areas.Authentication.Models
{
    public class SignUpModel
    {
        [Required]
        public string Name { get; set; }
        [Required]

        public string Email { get; set; }
        [Required]

        public string Password { get; set; }

        [ValidateNever]
        public string Role {  get; set; }
    }
}
