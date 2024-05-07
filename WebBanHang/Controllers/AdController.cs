using Microsoft.AspNetCore.Mvc;

namespace WebBanHang.Controllers
{
    public class AdController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
