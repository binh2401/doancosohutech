using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string author { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public int TotalLikes { get; set; }
        public string nhasanxuat { get; set; }
        public string congtyphathanh { get; set; }
        public string loaibia { get; set; }
        public int sotrang { get; set; }
        public double AverageRating { get; set; }
        public List<ProductImage>? Images { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public OrderDetail? OrderDetail { get; set; }   
        public Order? Order { get; set; }
        public bool IsLiked { get; set; }
        public ICollection<like> Likes { get; set; } = new List<like>();
        public ICollection<comment> Comments { get; set; } =new List<comment>();
        public int TotalReviews { get; set; } // Tổng số lượt đánh giá
        public int SoLuongBanRa { get; set; }
        public int LuongTonKho { get; set; }
      
        public void UpdateStock(int quantitySold)
        {
            SoLuongBanRa += quantitySold;
            LuongTonKho -= quantitySold;
        }
    }
}
