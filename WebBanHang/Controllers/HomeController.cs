using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using WebBanHang.Data;
using WebBanHang.Models;
using WebBanHang.Repositories;
using X.PagedList;



namespace WebBanHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;

        public readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IProductRepository productRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        // Hi?n th? danh sách s?n ph?m
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5;
            int pageNumber =page==null ||page<0?1:page.Value;
            var products = await _productRepository.GetAllAsync();
            PagedList<Product> lst=new PagedList<Product>(products, pageNumber, pageSize);
            return View(lst);
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

            // Lấy danh sách đánh giá của sản phẩm
            var reviews = await _context.comments
                                            .Where(r => r.ProductId == id)
                                            .ToListAsync();

            // Truyền danh sách đánh giá vào ViewBag để hiển thị trong view
            ViewBag.Reviews = reviews;

            // Truyền thông tin loại sản phẩm vào ViewBag
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
        public async Task<IActionResult> Productcategory(int categoryId, int? page)
        {
            // Define the page size
            int pageSize = 5;
            int pageNumber = page ?? 1;

            // Query the database to get products belonging to the specified category ID
            var productsInCategory = await _context.Products
                                                   .Where(p => p.CategoryId == categoryId)
                                                   .ToListAsync();

            // Create a PagedList object to handle pagination
            IPagedList<Product> pagedProducts = productsInCategory.ToPagedList(pageNumber, pageSize);

            // Pass categoryId and paginated products to the view
            ViewBag.CategoryId = categoryId;

            return View(pagedProducts);
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
        public async Task<IActionResult> likeuser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Truy vấn cơ sở dữ liệu để lấy danh sách các sản phẩm mà người dùng đã thích
            var likedProducts = await _context.Likes
                .Where(l => l.UserId == user.Id)
                .Select(l => l.Product)
                .ToListAsync();

            return View(likedProducts);
        }
        public async Task<IActionResult> ProductcategoryByMenu(int menuId, int? page)
        {
            // Define the page size
            int pageSize = 5;
            int pageNumber = page ?? 1;

            // Query the database to get products belonging to the specified menu ID
            var productsInMenu = await _context.Products
                                               .Where(p => p.Category.menu.Id == menuId && p.LuongTonKho > 0)
                                               .ToListAsync();

            // Create a PagedList object to handle pagination
            IPagedList<Product> pagedProducts = productsInMenu.ToPagedList(pageNumber, pageSize);

            // Pass menuId and paginated products to the view
            ViewBag.MenuId = menuId;

            return View("Productcategory", pagedProducts);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int productId, int rating, string reviewText)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Người dùng chưa đăng nhập, bạn có thể xử lý điều này tại đây
                return RedirectToAction("Login", "Account");
            }

            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = HttpContext.User.Identity.Name; // Lấy tên người dùng

            // Tạo một đối tượng Review mới
            var review = new comment
            {
                ProductId = productId,
                name= userName,
                Rating = rating,
                ReviewText = reviewText,
                UserId = userId
            };

            // Thêm đánh giá vào cơ sở dữ liệu
            _context.comments.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", new { id = productId });
        }
        public async Task<IActionResult> SortByPriceAsc(int? page)
        {
            var products = _context.Products.OrderBy(p => p.Price).ToList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View("Productcategory", products.ToPagedList(pageNumber, pageSize));
        }
        public async Task<IActionResult> giamPriceAsc(int? page)
        {
            var products = _context.Products.OrderByDescending(p => p.Price).ToList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View("Productcategory", products.ToPagedList(pageNumber, pageSize));
        }
        public async Task< IActionResult> FilterByLetter(char letter)
        {
            var products = _context.Products
                                   .Where(p => p.Name.StartsWith(letter.ToString()))
                                   .ToList();
            return View("Productcategory", products);
        }
        public async Task <IActionResult> FilterByLetterztoa(char letter)
        {
            var products = _context.Products
                           .Where(p => p.Name.StartsWith(letter.ToString()))
                           .OrderByDescending(p => p.Name) // Sắp xếp theo thứ tự giảm dần của tên
                           .ToList();
            return View("Productcategory", products);
        }
        public async Task< IActionResult> BestSellingProducts()
        {
            var bestSellingProducts = _context.Products
                .OrderByDescending(p => p.SoLuongBanRa)
                .Take(10) // Lấy 10 sản phẩm bán chạy nhất
                .ToList();

            return View(bestSellingProducts);
        }
        public async Task< IActionResult> MostLikedProducts()
        {
            var mostLikedProducts = _context.Products
                .OrderByDescending(p => p.TotalLikes)
                .Take(10) // Lấy 10 sản phẩm được yêu thích nhất
                .ToList();

            return View("BestSellingProducts",mostLikedProducts);
        }

    }
}

    
