using WebBanHang.Models;

namespace WebBanHang.Repositories
{
    public interface IMenu
    {
        Task<IEnumerable<Menu>> GetAllAsync();
        Task<Menu> GetByIdAsync(int id);
        Task AddAsync(Menu menu);
        Task UpdateAsync(Menu menu);
        Task DeleteAsync(int id);
    }
}
