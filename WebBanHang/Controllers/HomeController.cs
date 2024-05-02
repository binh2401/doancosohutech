using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;

        public HomeController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
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
    }
}
