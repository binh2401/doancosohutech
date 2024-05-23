using Microsoft.EntityFrameworkCore;

using WebBanHang.Repositories;

using WebBanHang.Models;
using WebBanHang.Data;

public class EFProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    public EFProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
         .Include(p => p.Category) // Bao gồm thông tin về danh mục (category)
             .ThenInclude(c => c.menu) // Bao gồm thông tin về menu
         .ToListAsync();
    }
    public async Task<Product> GetProductByIdAsync(int productId)
    {
        var product = await _context.Products
            .Include(p => p.Category) // Bao gồm cả category
            .FirstOrDefaultAsync(p => p.Id == productId);

        return product; // Trả về sản phẩm, kể cả khi không có category
    }
    public async Task<Product> GetByIdAsync(int id)
    {
        // return await _context.Products.FindAsync(id);
        // lấy thông tin kèm theo category
        return await _context.Products
         .Include(p => p.Category) // Bao gồm thông tin về category
         .ThenInclude(c => c.menu) // Bao gồm thông tin về menu
         .FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
    public IEnumerable<Product> GetAllProducts()
    {
        return _context.Products.ToList();
    }

    public IEnumerable<Product> Searchsach(string searchTerm)
    {

        // Truy vấn các sản phẩm có tên chứa searchTerm từ cơ sở dữ liệu
        return _context.Products.Where(p => p.Name.Contains(searchTerm)).ToList();

    }
}