using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models;

namespace WebBanHang.Repositories
{
    public class EFMenu: IMenu
    {
        private readonly ApplicationDbContext _context;
        public EFMenu(ApplicationDbContext context)
        {
            _context = context;
        }
        // Tương tự như EFProductRepository, nhưng cho Category
        public async Task<IEnumerable<Menu>> GetAllAsync()
        {
            // return await _context.Products.ToListAsync();
            return await _context.Menus.Include(p=>p.Categorys).ToListAsync();
        }
        public async Task<Menu> GetByIdAsync(int id)
        {
            // return await _context.Products.FindAsync(id);
            // lấy thông tin kèm theo category
            return await _context.Menus.Include(p => p.Categorys).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task AddAsync(Menu menucategory)
        {
            _context.Menus.Add(menucategory);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Menu menucategory)
        {
            _context.Menus.Update(menucategory);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {


            var menucategory = await _context.Menus.FindAsync(id);
            _context.Menus.Remove(menucategory);
            await _context.SaveChangesAsync();
        }
    }
}
