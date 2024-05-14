using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models
{
    public class AccountViewModel
    {
        public ApplicationUser User { get; set; }
        public IList<string> Roles { get; set; }
        [Key]
        public int Id { get; set; }
        
    }
}
