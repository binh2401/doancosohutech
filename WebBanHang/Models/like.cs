namespace WebBanHang.Models
{
    public class like
    {
        public int Id { get; set; }
        public bool IsLiked { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }

        public Product Product { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}
