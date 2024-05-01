using WebBanHang.Models;

namespace WebBanHang.Models
{
    public interface IProductRepository
    {
        IEnumerable<Product> Searchsach(string searchTerm);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
