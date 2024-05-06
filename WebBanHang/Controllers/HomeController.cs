using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebBanHang.Models;
using WebBanHang.Repositories;

namespace WebBanHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
      
        public readonly ApplicationDbContext _context;

        public HomeController(IProductRepository productRepository, ApplicationDbContext context )
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
        public async Task<IActionResult> LikeProduct(int id)
        {
            // Lấy sản phẩm từ cơ sở dữ liệu bằng productId
            var product = await _productRepository.GetByIdAsync(id);

            if (product != null)
            {
                // Tăng số lượng "Like" của sản phẩm
                product.like++;
                await _productRepository.UpdateAsync(product); // Lưu thay đổi vào cơ sở dữ liệu
            }
            else
            {
                return NotFound();
            }
            // Chuyển hướng về trang chi tiết sản phẩm
            return Ok();
        }
        public async Task<IActionResult> countbuy(int id)
        {
            // Lấy sản phẩm từ cơ sở dữ liệu bằng productId
            var product = await _productRepository.GetByIdAsync(id);

            if (product != null)
            {
                // Tăng số lượng "Like" của sản phẩm
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
            return View(product);
        }
        public async Task<IActionResult> MenuPartial()
        {
            var listmenu= _context.Menus.ToList();

            return PartialView();
        }
    }
}
