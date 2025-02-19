namespace BOXCricket.Models
{
    public class DashboardModel
    {
        public List<CountModel> Counts { get; set; }
        public List<RecentBookingsModel> Bookings { get; set; }
        public List<RecentPaymentsModel> Payments { get; set; }
    }

    public class CountModel
    {
        public string Metric { get; set; }
        public int Value { get; set; }
    }

    public class RecentBookingsModel
    {
        public int BookingID { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string VenueName { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime CreatedAt { get; set; }

    }

    public class RecentPaymentsModel
    {
        public int PaymentId { get; set; }
        public string UserName { get; set; }
        public string VenueName { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }

    }
}
