using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Models
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Order Order { get; set; }
        public List<Product> Product { get; set; }

    }
}
