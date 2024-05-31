namespace WebBanHang.Models
{
    public class RevenueStatisticsViewModel
    {
        public List<RevenueStatistics> RevenueData { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
