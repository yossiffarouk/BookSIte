using BookSIte.Data;
using BookSIte.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookSIte.Controllers
{
    public class CategoryController : Controller
    {
        private readonly Context _Context;

        public CategoryController(Context Context)
        {
            _Context = Context;
            
        }


        public IActionResult Index()
        {
            var Categorys = _Context.Category.ToList();
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
                    _Context.Category.Add(CAT);
                    _Context.SaveChanges();
                     TempData["Create"] = "New Category Added";
                     return  RedirectToAction("Index");

            }
                    return View();
        }

        public IActionResult Edit(int id)
        {
            if (id == null)
            {
                return NotFound(); 
            }
            var Category = _Context.Category.Find(id);
            return View(Category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
           
            if (ModelState.IsValid)
            {
                _Context.Category.Update(category);
                _Context.SaveChanges();
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
            var Category = _Context.Category.Find(id);
            TempData["Delete"] = $"{Category.Name} Has Deleteded";
            _Context.Category.Remove(Category);
            _Context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
