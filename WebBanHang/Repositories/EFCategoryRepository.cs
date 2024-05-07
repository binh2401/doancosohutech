using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models;
using WebBanHang.Repositories;


public class EFCategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;
    public EFCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    // Tương tự như EFProductRepository, nhưng cho Category
    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        // return await _context.Products.ToListAsync();
        return await _context.Categories
        .Include(p => p.Products) // Include thông tin về product
        .ToListAsync();
    }
    public async Task<Category> GetByIdAsync(int id)
    {
        // return await _context.Products.FindAsync(id);
        // lấy thông tin kèm theo category
        return await _context.Categories.Include(p => p.Products).FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task AddAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)
    {


        var category = await _context.Categories.FindAsync(id);
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

  

}