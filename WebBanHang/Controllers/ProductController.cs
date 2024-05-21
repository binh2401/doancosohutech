using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebBanHang.Models;
using WebBanHang.Repositories;

namespace WebBanHang.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductController(IProductRepository productRepository,
        ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
{
var products = await _productRepository.GetAllAsync();
return View(products);
    }
    // Hiển thị form thêm sản phẩm mới
    public async Task<IActionResult> Add()
    {
        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View();
    }
    // Xử lý thêm sản phẩm mới
    [HttpPost]
    public async Task<IActionResult> Add(Product product, IFormFile
    imageUrl)
    {
        if (ModelState.IsValid)
        {
            if (imageUrl != null)
            {
                // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage

                product.ImageUrl = await SaveImage(imageUrl);

            }
            await _productRepository.AddAsync(product);
            return RedirectToAction(nameof(Index));
        }
        // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(product);
    }
    // Viết thêm hàm SaveImage (tham khảo bài 02)
    private async Task<string> SaveImage(IFormFile image)
    {
        var savePath = Path.Combine("wwwroot/images", image.FileName); //Thay đổi đường dẫn theo cấu hình của bạn
    using (var fileStream = new FileStream(savePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }
        return "/images/" + image.FileName; // Trả về đường dẫn tương đối
    }
//Nhớ tạo folder images trong wwwroot

// Hiển thị thông tin chi tiết sản phẩm
public async Task<IActionResult> Display(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }
    // Hiển thị form cập nhật sản phẩm
    public async Task<IActionResult> Update(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name",
        product.CategoryId);
        return View(product);
    }
    // Xử lý cập nhật sản phẩm
    [HttpPost]
    public async Task<IActionResult> Update(int id, Product product,
    IFormFile imageUrl)
    {
        ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
    if (id != product.Id)
        {
            return NotFound();
        }
        if (ModelState.IsValid)
        {
            var existingProduct = await
            _productRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync
                                                 // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
        if (imageUrl == null)
            {
                product.ImageUrl = existingProduct.ImageUrl;
            }
            else
            {
                // Lưu hình ảnh mới

                product.ImageUrl = await SaveImage(imageUrl);

            }
            // Cập nhật các thông tin khác của sản phẩm
        
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.author=product.author;
            existingProduct.nhasanxuat= product.nhasanxuat; 
            existingProduct.congtyphathanh= product.congtyphathanh;
            existingProduct.loaibia= product.loaibia;   
            existingProduct.sotrang= product.sotrang;
            existingProduct.SoLuongBanRa= product.SoLuongBanRa;
            existingProduct.LuongTonKho= product.LuongTonKho;
            existingProduct.TotalLikes= product.TotalLikes;
            await _productRepository.UpdateAsync(existingProduct);
            return RedirectToAction(nameof(Index));
        }
        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(product);
    }
    // Hiển thị form xác nhận xóa sản phẩm
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }
    // Xử lý xóa sản phẩm
    [HttpPost, ActionName("DeleteConfirmed")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _productRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
        public IActionResult Search(string searchTerm)
        {
            // Gọi phương thức tìm kiếm từ repository
            IEnumerable<Product> searchResults = _productRepository.Searchsach(searchTerm);

            // Trả về view với kết quả tìm kiếm
            return View("SearchResults", searchResults);
        }

        public async Task<IActionResult> SortByName()
        {
            var sortedProducts = await _productRepository.GetAllAsync();
            sortedProducts = sortedProducts.OrderBy(p => p.Name);
            return View("Index", sortedProducts);
        }

        public async Task<IActionResult> SortByPriceAsc()
        {
            var sortedProducts = await _productRepository.GetAllAsync();
            sortedProducts = sortedProducts.OrderBy(p => p.Price);
            return View("Index", sortedProducts);
        }
        public async Task<IActionResult> giamPriceAsc()
        {
            var sortedProducts = await _productRepository.GetAllAsync();
            sortedProducts = sortedProducts.OrderByDescending(p => p.Price);
            return View("Index", sortedProducts);
        }

    }
    }

