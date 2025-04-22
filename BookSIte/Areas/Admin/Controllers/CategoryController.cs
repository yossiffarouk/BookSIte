using BookkStore.Utility;
using BookSite.DataAccess.Repository.Category;
using BookSite.DataAccess.Repository.Unitofwork;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Category = BookStore.Models.Category;




namespace BookSIte.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_User_Admin)]
    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _Unit;

        public CategoryController(IUnitOfWork Unit)
        {
            _Unit = Unit;

        }


        public IActionResult Index()
        {
            var Categorys = _Unit.CategoryRepo.GetAll();
            return View(Categorys);
        }


        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Category CAT)
        {
            if (ModelState.IsValid)
            {
                _Unit.CategoryRepo.Add(CAT);
                _Unit.savechanges();
                TempData["Create"] = "New Category Added";
                return RedirectToAction("Index");

            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Category = _Unit.CategoryRepo.Get(a => a.Id == id);
            return View(Category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {

            if (ModelState.IsValid)
            {
                _Unit.CategoryRepo.Update(category);
                _Unit.savechanges();
                TempData["Update"] = $"Category Has Updateded";
                return RedirectToAction("Index");

            }
            return View();
        }


        public IActionResult Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Category = _Unit.CategoryRepo.Get(a => a.Id == id);
            TempData["Delete"] = $"{Category.Name} Has Deleteded";
            _Unit.CategoryRepo.Remove(Category);
            _Unit.savechanges();
            return RedirectToAction("Index");
        }
    }
}
