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
        public async Task<IActionResult> Index()
        {
            var productdf = await _productRepository.GetAllAsync();
            var products = _context.Products.Include(p => p.Likes).ToList();
            var productLikes = new Dictionary<int, bool>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var product in products)
            {
                productLikes[product.Id] = _context.Likes.Any(l => l.ProductId == product.Id && l.UserId == userId);
            }
         
            ViewBag.IsLiked = productLikes;
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
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            //var productCategory = await _productRepository.GetByIdAsync(product.CategoryId);
            //if (productCategory == null)
            //{
            //    return NotFound("Product category not found");
            //}

            // Lấy danh sách đánh giá của sản phẩm
            var reviews = await _context.comments
                                            .Where(r => r.ProductId == id)
                                            .ToListAsync();

            // Truyền danh sách đánh giá vào ViewBag để hiển thị trong view
            ViewBag.Reviews = reviews;

            // Truyền thông tin loại sản phẩm vào ViewBag
            //ViewBag.ProductCategory = productCategory;

            return View(product);
        }

        public async Task<IActionResult> MenuPartial()
        {
            var listmenu = _context.Menus.ToList();

            return PartialView();
        }
        [HttpPost]
        public async Task<IActionResult> ProductsByCategory(int categoryId)
        {
            var products = _context.Products.Include(p => p.Likes).ToList();
            var productLikes = new Dictionary<int, bool>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var product in products)
            {
                productLikes[product.Id] = _context.Likes.Any(l => l.ProductId == product.Id && l.UserId == userId);
            }

            ViewBag.IsLiked = productLikes;
            // Lấy danh sách sản phẩm thuộc thể loại categoryId từ cơ sở dữ liệu
            var productsInCategory = await _context.Products
               .Where(p => p.CategoryId == categoryId)
               .ToListAsync();

            return View(productsInCategory);
        }

        public async Task<IActionResult> Productcategory(int categoryId)
        {
            var productdf = await _productRepository.GetAllAsync();
            // Define the page size
            var products = _context.Products.Include(p => p.Likes).ToList();
            var productLikes = new Dictionary<int, bool>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var product in products)
            {
                productLikes[product.Id] = _context.Likes.Any(l => l.ProductId == product.Id && l.UserId == userId);
            }

            ViewBag.IsLiked = productLikes;
            // Query the database to get products belonging to the specified category ID
            var productsInCategory = await _context.Products
                                                   .Where(p => p.CategoryId == categoryId)
                                                   .ToListAsync();

            // Create a PagedList object to handle pagination

            // Pass categoryId and paginated products to the view
            ViewBag.CategoryId = categoryId;


            return View(productsInCategory);
        }
        [HttpPost]
        public JsonResult LikeProduct(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập" });
            }
            var product = _context.Products.Find(productId);

            if (product == null)
            {
                return Json(new { success = false, message = "hiện không có sản phẩm" });
            }

            var existingLike = _context.Likes.FirstOrDefault(l => l.ProductId == productId && l.UserId == userId);
            if (existingLike != null)
            {
                _context.Likes.Remove(existingLike);
                product.TotalLikes -= 1;
            }
            else
            {
                var like = new like { ProductId = productId, UserId = userId };
                _context.Likes.Add(like);
                product.TotalLikes += 1;
            }

            _context.SaveChanges();

            var isLiked = _context.Likes.Any(l => l.ProductId == productId && l.UserId == userId);

            return Json(new { success = true, totalLikes = product.TotalLikes, isLiked = isLiked });
        }

        public bool IsLikedByUser(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return _context.Likes.Any(l => l.ProductId == productId && l.UserId == userId);
        }
        
        public async Task<IActionResult> likeuser()
        {
            var productdf = await _productRepository.GetAllAsync();
            var products = _context.Products.Include(p => p.Likes).ToList();
            var productLikes = new Dictionary<int, bool>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập" });
            }

            foreach (var product in products)
            {
                productLikes[product.Id] = _context.Likes.Any(l => l.ProductId == product.Id && l.UserId == userId);
            }

            ViewBag.IsLiked = productLikes;
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
        public async Task<IActionResult> ProductcategoryByMenu(int menuId)
        {
            // Define the page size
            var productdf = await _productRepository.GetAllAsync();
            // Query the database to get products belonging to the specified menu ID
            var productsInMenu = await _context.Products
                                               .Where(p => p.Category.menu.Id == menuId && p.LuongTonKho > 0)
                                               .ToListAsync();
            // Create a PagedList object to handle pagination
          
            // Pass menuId and paginated products to the view
            ViewBag.MenuId = menuId;
            return View("Productcategory", productsInMenu);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int productId, int rating, string reviewText)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Người dùng chưa đăng nhập, bạn có thể xử lý điều này tại đây
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện" });
            }

            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = HttpContext.User.Identity.Name; // Lấy tên người dùng

            // Tạo một đối tượng Review mới
            var review = new comment
            {
                ProductId = productId,
                name = userName,
                Rating = rating,
                ReviewText = reviewText,
                UserId = userId
            };

            // Thêm đánh giá vào cơ sở dữ liệu
            _context.comments.Add(review);
            await _context.SaveChangesAsync();

            // Sau khi thêm đánh giá mới, tính lại tổng trung bình của đánh giá
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                var comments = await _context.comments.Where(c => c.ProductId == productId).ToListAsync();
                double totalRating = comments.Sum(c => (double)c.Rating);
                int totalComments = comments.Count();

                product.AverageRating = (double)(totalRating / totalComments);
                product.TotalReviews = totalComments;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Detail", new { id = productId });
        }


        public async Task<IActionResult> SortByPriceAsc()
        {
            var products = _context.Products.OrderBy(p => p.Price).ToList();
         
            return View("Productcategory", products);
        }
        public async Task<IActionResult> giamPriceAsc()
        {
            var products = _context.Products.OrderByDescending(p => p.Price).ToList();
            return View("Productcategory", products);
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
            var products = _context.Products.Include(p => p.Likes).ToList();
            var productLikes = new Dictionary<int, bool>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var product in products)
            {
                productLikes[product.Id] = _context.Likes.Any(l => l.ProductId == product.Id && l.UserId == userId);
            }

            ViewBag.IsLiked = productLikes;
            var bestSellingProducts = _context.Products
                .OrderByDescending(p => p.SoLuongBanRa)
                .Take(20) // Lấy 10 sản phẩm bán chạy nhất
                .ToList();

            return View(bestSellingProducts);
        }
        public async Task< IActionResult> MostLikedProducts()
        {
            var products = _context.Products.Include(p => p.Likes).ToList();
            var productLikes = new Dictionary<int, bool>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var product in products)
            {
                productLikes[product.Id] = _context.Likes.Any(l => l.ProductId == product.Id && l.UserId == userId);
            }

            ViewBag.IsLiked = productLikes;
            var mostLikedProducts = _context.Products
                .OrderByDescending(p => p.TotalLikes)
                .Take(20)// Lấy 10 sản phẩm được yêu thích nhất
                .ToList();

            return View("BestSellingProducts",mostLikedProducts);
        }

    }
}

    
