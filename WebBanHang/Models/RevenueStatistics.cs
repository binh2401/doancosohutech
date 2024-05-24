namespace WebBanHang.Models
{
    public class RevenueStatistics
    {
        public int Id { get; set; } // Khóa chính
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
    }
}
