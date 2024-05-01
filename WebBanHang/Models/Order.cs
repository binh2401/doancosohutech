using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Models
{
    [Table("Order")]
    public class Order
    {
        [Required(ErrorMessage = "Shipping Address is required")]
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public int count { get; set; }
        public decimal TotalPrice { get; set; }
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Notes is required")]
        public string Notes { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
