using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models;
using WebBanHang.Repositories;

namespace WebBanHang.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenu _imenu;
        private readonly ICategoryRepository _categoryRepository;
        public MenuController(IMenu Imenu, ICategoryRepository
        categoryRepository)
        {
            _imenu = Imenu;
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index()
        {
            var menucategory = await _imenu.GetAllAsync();
            return View(menucategory);
        }
        public async Task<IActionResult> Display(int id)
        {
            var menucategory = await _imenu.GetByIdAsync(id);
            if (menucategory == null)
            {
                return NotFound();
            }
            return View(menucategory);
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Menu menuCategory)
        {
            if (ModelState.IsValid)
            {
                await _imenu.AddAsync(menuCategory);
                return RedirectToAction(nameof(Index));
            }
            return View(menuCategory);
        }
        public async Task<IActionResult> Update(int id)
        {
            var menucategory = await _imenu.GetByIdAsync(id);
            if (menucategory == null)
            {
                return NotFound();
            }
            return View(menucategory);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Menu menuCategory)
        {
            if (id != menuCategory.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _imenu.UpdateAsync(menuCategory);
                return RedirectToAction(nameof(Index));

            }
            return View(menuCategory);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var menucategory = await _imenu.GetByIdAsync(id);
            if (menucategory == null)
            {
                return NotFound();
            }
            return View(menucategory);
        }
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menucategory = await _imenu.GetByIdAsync(id);
            if (menucategory != null)
            {
                await _categoryRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
