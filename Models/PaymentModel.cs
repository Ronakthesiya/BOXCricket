using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOXCricket.Models
{
    public class PaymentModel
    {
        [Key]
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int VenueId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = "Net Banking";
        public string TransactionId { get; set; }
        public string Status { get; set; } = "Success";

        [ValidateNever]
        public UserModel User { get; set; }

        [ValidateNever]
        public VenueModel Venue { get; set; }

    }
}
