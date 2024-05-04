using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Range(0.01, 10000.00)]
    
        public decimal Price { get; set; }
        public string author { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public int like { get; set; }
        public string nhasanxuat { get; set; }
        public string congtyphathanh { get; set; }
        public string loaibia { get; set; }
        public int sotrang { get; set; }
        public int countbuy { get; set; }
        public List<ProductImage>? Images { get; set; }
        public int CategoryId { get; set; }
        public int Catagorytong { get; set; }
        public Category? Category { get; set; }
        public OrderDetail? OrderDetail { get; set; }   
        public Order? Order { get; set; }
     
    }
}
