using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using WebBanHang.Data;
using WebBanHang.Models;
using WebBanHang.Repositories;

namespace WebBanHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;

        public readonly ApplicationDbContext _context;

        public HomeController(IProductRepository productRepository, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;

        }

        // Hi?n th? danh sách s?n ph?m
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> CountBuy(int id)
        {
            // Lấy sản phẩm từ cơ sở dữ liệu bằng productId
            var product = await _productRepository.GetByIdAsync(id);

            if (product != null)
            {
                // Tăng số lượng "CountBuy" của sản phẩm
                product.countbuy++;
                await _productRepository.UpdateAsync(product); // Lưu thay đổi vào cơ sở dữ liệu
            }
            else
            {
                return NotFound();
            }
            // Chuyển hướng về trang chi tiết sản phẩm
            return Ok();
        }
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var productCategory = await _productRepository.GetByIdAsync(product.CategoryId);

            if (productCategory == null)
            {
                return NotFound("Product category not found");
            }

            // Sử dụng ViewBag để truyền thông tin loại sản phẩm vào view
            ViewBag.ProductCategory = productCategory;

            return View(product);
        }
        public async Task<IActionResult> MenuPartial()
        {
            var listmenu = _context.Menus.ToList();

            return PartialView();
        }
        public async Task<IActionResult> ProductsByCategory(int categoryId)
        {
            // Lấy danh sách sản phẩm thuộc thể loại categoryId từ cơ sở dữ liệu
            var productsInCategory = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            return View(productsInCategory);
        }
        public async Task<IActionResult> Productcategory(int categoryId)
        {
            // Truy vấn cơ sở dữ liệu để lấy các sản phẩm thuộc danh mục có ID là categoryId
            var productsInCategory = await _productRepository.GetAllAsync();
            productsInCategory = await _context.Products
                                                 .Where(p => p.CategoryId == categoryId)
                                                 .ToListAsync();

            // Trả về view và truyền danh sách sản phẩm tới view
            return View(productsInCategory);
        }
        public async Task<IActionResult> Productmenu(int categoryId)
        {
            // Lấy danh sách các danh mục thuộc thể loại lớn (categoryId) từ cơ sở dữ liệu
            var categoriesInMainCategory = await _context.Categories
                                                .Where(c => c.menuid == categoryId)
                                                .Select(c => c.Id)
                                                .ToListAsync();

            // Lấy danh sách sản phẩm thuộc các danh mục trong danh sách danh mục đã lấy được
            var productsInMainCategory = await _context.Products
                                                      .Where(p => categoriesInMainCategory.Contains(p.CategoryId))
                                                      .ToListAsync();

            // Trả về view và truyền danh sách sản phẩm tới view
            return View(productsInMainCategory);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikeProduct(int productId)
        {
            // Lấy sản phẩm
            var product = await _context.Products.FindAsync(productId);

            // Kiểm tra sản phẩm tồn tại
            if (product == null)
            {
                return NotFound();
            }

            // Thêm "like" cho sản phẩm
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "User not logged in." });
            }

            // Kiểm tra xem đã tồn tại "like" từ người dùng cho sản phẩm này chưa
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.ProductId == productId && l.UserId == userId);

            if (existingLike == null)
            {
                var newLike = new like
                {
                    ProductId = productId,
                    UserId = userId,
                    IsLiked = true
                };

                // Thêm "like" mới vào cơ sở dữ liệu
                _context.Likes.Add(newLike);
                await _context.SaveChangesAsync();

                // Cập nhật tổng số lượt "like" của sản phẩm
                product.TotalLikes++;
                await _context.SaveChangesAsync();

                return Json(new { success = true, totalLikes = product.TotalLikes });
            }

            return Json(new { success = false, message = "User has already liked this product." });
        }
        public async Task<IActionResult> UnlikeProduct(int productId)
{
    // Lấy sản phẩm
    var product = await _context.Products.FindAsync(productId);

    // Kiểm tra sản phẩm tồn tại
    if (product == null)
    {
        return NotFound();
    }

    // Xóa "like" cho sản phẩm
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId == null)
    {
        return Json(new { success = false, message = "User not logged in." });
    }

    var existingLike = await _context.Likes
        .FirstOrDefaultAsync(l => l.ProductId == productId && l.UserId == userId);

    if (existingLike != null)
    {
        // Xóa "like" khỏi cơ sở dữ liệu
        _context.Likes.Remove(existingLike);
        await _context.SaveChangesAsync();

        // Giảm tổng số lượt "like" của sản phẩm
        product.TotalLikes--;
        await _context.SaveChangesAsync();

        return Json(new { success = true, totalLikes = product.TotalLikes });
    }

    return Json(new { success = false, message = "User has not liked this product." });
}


    }
}

    
