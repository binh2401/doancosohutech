using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace WebBanHang.Models
{
    public class comment
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public string name { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Các mối quan hệ nếu có
        public Product Product { get; set; }
        public ApplicationUser User { get; set; }
    
    }
}
