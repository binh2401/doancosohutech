using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using WebBanHang.Data;
using WebBanHang.Extensions;
using WebBanHang.Models;
using WebBanHang.Services;

namespace WebBanHang.Controllers
{
	[Authorize]
	public class ShoppingCartController : Controller
	{
		private readonly IProductRepository _productRepository;
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IVnPayService _vnPayservice;

		public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository productRepository, IVnPayService vnPayService)
		{
			_productRepository = productRepository;
			_context = context;
			_userManager = userManager;
			_vnPayservice = vnPayService;

		}

		public async Task<IActionResult> AddToCart(int productId, int quantity)
		{
			// Giả sử bạn có phương thức lấy thông tin sản phẩm từ productId
			var product = await GetProductFromDatabase(productId);

			var cartItem = new CartItem
			{
				ProductId = productId,
				Name = product.Name,
				ImageUrl = product.ImageUrl,
				Price = product.Price,

				Quantity = quantity
			};

			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
			cart.AddItem(cartItem);

			HttpContext.Session.SetObjectAsJson("Cart", cart);

			return RedirectToAction("Index");
		}

		public IActionResult Index()
		{
			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
			return View(cart);
		}

		// Các actions khác...

		private async Task<Product> GetProductFromDatabase(int productId)
		{
			// Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm
			var product = await _productRepository.GetByIdAsync(productId);
			return product;
		}
		public IActionResult Checkout()
		{
			return View(new Order());
		}

		[HttpPost]
		public async Task<IActionResult> Checkout(Order order, string payment = "COD")
		{
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (payment == "Thanh toán VNPay")
			{
				var vnPayModel = new VnPaymentRequestModel
				{
					Amount = cart.Items.Sum(i=> (double)(i.Price*1000) *i.Quantity),
					CreatedDate = DateTime.Now,
					Description =order.Notes ,
					FullName = order.ShippingAddress,
					OrderId = order.Id
				};
                var ser = await _userManager.GetUserAsync(User);
                order.UserId = ser.Id;
                order.OrderDate = DateTime.UtcNow;
                order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
                int totalCount = cart.Items.Sum(i => i.Quantity);
                order.count = totalCount;
                order.OrderDetails = cart.Items.Select(i => new OrderDetail
				
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList();

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("Cart");
                return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
			}
		
			if (cart == null || !cart.Items.Any())
			{
				// Xử lý giỏ hàng trống...
				return RedirectToAction("Index");
			}

			var user = await _userManager.GetUserAsync(User);
			order.UserId = user.Id;
			order.OrderDate = DateTime.UtcNow;
			order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            int otalCount = cart.Items.Sum(i => i.Quantity);
            order.count = otalCount;
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
			{
				ProductId = i.ProductId,
				Quantity = i.Quantity,
				Price = i.Price
			}).ToList();
            _context.Orders.Add(order);
			await _context.SaveChangesAsync();
			HttpContext.Session.Remove("Cart");

			return View("OrderCompleted", order.Id); // Trang xác nhận hoàn thành đơn hàng
		}

		public IActionResult RemoveFromCart(int productId)
		{
			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

			if (cart is not null)
			{
				cart.RemoveItem(productId);

				// Lưu lại giỏ hàng vào Session sau khi đã xóa mục
				HttpContext.Session.SetObjectAsJson("Cart", cart);
			}

			return RedirectToAction("Index");
		}

		// Action để xóa toàn bộ mặt hàng trong giỏ hàng
		[HttpPost]
		public IActionResult ClearCart()
		{
			HttpContext.Session.Remove("Cart");
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult Increase(int productId)
		{
			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
			if (cart != null)
			{
				var cartItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);
				if (cartItem != null)
				{
					cartItem.Quantity++;
					HttpContext.Session.SetObjectAsJson("Cart", cart);
				}
			}
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult Decrease(int productId)
		{
			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
			if (cart != null)
			{
				var cartItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);
				if (cartItem != null)
				{
					if (cartItem.Quantity > 1)
					{
						cartItem.Quantity--;
					}
					else
					{
						cart.Items.Remove(cartItem);
					}
					HttpContext.Session.SetObjectAsJson("Cart", cart);
				}
			}
			return RedirectToAction("Index");
		}
		public IActionResult PaymentCallBack()
		{
			var response = _vnPayservice.PaymentExecute(Request.Query);

			if (response == null || response.VnPayResponseCode != "00")
			{
				TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
				return RedirectToAction("PaymentFail");
			}


			// Lưu đơn hàng vô database

			TempData["Message"] = $"Thanh toán VNPay thành công";
			return RedirectToAction("PaymentSuccess");
		}
		public IActionResult PaymentSuccess()
		{
			return View("Success");
		}
		public IActionResult PaymentFail()
		{
			return View();
		}
	}
}