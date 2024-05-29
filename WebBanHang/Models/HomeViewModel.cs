using PagedList;

namespace WebBanHang.Models
{
    public class HomeViewModel
    {
        public IPagedList<Product> Products { get; set; }
        public Dictionary<int, bool> IsLikedDict { get; set; } // Thêm dictionary để lưu trạng thái "like"
    }
}
