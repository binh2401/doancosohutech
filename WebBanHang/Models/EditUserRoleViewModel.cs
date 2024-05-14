using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models
{
    public class EditUserRoleViewModel
    {
        public string UserId { get; set; }
        public List<string> UserRoles { get; set; } = new List<string>();
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> SelectedRoles { get; set; } = new List<string>(); // Đảm bảo rằng danh sách này được khởi tạo
    }
}
