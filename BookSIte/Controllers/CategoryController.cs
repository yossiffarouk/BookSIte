using BookSite.DataAccess.Repository.Category;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Category = BookStore.Models.Category;




namespace BookSIte.Controllers
{
    public class CategoryController : Controller
    {
        
        private readonly ICategoryRepo _category;

        public CategoryController(ICategoryRepo category)
        {
            _category = category;
            
        }


        public IActionResult Index()
        {
            var Categorys = _category.GetAll();
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
                     _category.Add(CAT);
                     _category.savechanges();
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
            var Category = _category.Get( a=> a.Id == id);
            return View(Category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
           
            if (ModelState.IsValid)
            {
                _category.Update(category);
                _category.savechanges();
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
            var Category = _category.Get(a=> a.Id == id );
            TempData["Delete"] = $"{Category.Name} Has Deleteded";
            _category.Remove(Category);
            _category.savechanges();
            return RedirectToAction("Index");
        }
    }
}
