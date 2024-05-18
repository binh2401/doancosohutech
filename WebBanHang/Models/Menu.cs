using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models
{
    public class Menu
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        public List<Category>? Categorys { get; set; }
      
    }
}
