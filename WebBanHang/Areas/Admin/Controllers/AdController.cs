using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models;
using WebBanHang.Repositories;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdController : Controller
    {


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Account()
        {
            var users = await _userManager.Users.ToListAsync();

            // Tạo một danh sách view model để lưu trữ thông tin người dùng kèm chức vụ
            var usersViewModel = new List<AccountViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Tạo một view model cho từng người dùng
                var userViewModel = new AccountViewModel
                {
                    User = user,
                    Roles = roles
                };

                usersViewModel.Add(userViewModel);
            }

            return View(usersViewModel);
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> EditUserRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var allRoles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);
            var model = new EditUserRoleViewModel
            {
                UserId = user.Id,
                UserRoles = userRoles.ToList(),
                Roles = allRoles.Select(r => r.Name).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        
        public async Task<IActionResult> EditUserRole(EditUserRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                var currentRoles = await _userManager.GetRolesAsync(user);

                // Remove all current roles
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to remove user roles");
                    return View(model);
                } 

                // Add the selected roles
                var addResult = await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to add selected roles to user");
                    return View(model);
                }

                return RedirectToAction("Account"); // Redirect to the user list page after a successful update
            }

            // If the data is invalid, display the form again with the entered data and error messages
            var allRoles = await _roleManager.Roles.ToListAsync();
            model.Roles = allRoles.Select(r => r.Name).ToList();
            model.UserRoles = (await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(model.UserId))).ToList();
            return View(model);
        }
        public async Task<IActionResult> danhthu(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.revenueStatistics.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(r => r.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.Date <= endDate.Value);
            }

            var revenueData = await query
                .GroupBy(r => r.Date)
                .Select(g => new RevenueStatistics
                {
                    Date = g.Key,
                    Revenue = g.Sum(r => r.Revenue)
                })
                .ToListAsync();

            var viewModel = new RevenueStatisticsViewModel
            {
                RevenueData = revenueData,
                StartDate = startDate,
                EndDate = endDate
            };

            return View(viewModel);
        }
      


}
}
