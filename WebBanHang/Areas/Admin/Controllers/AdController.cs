using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class AdController : Controller
    {
        [Area("Admin")]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
