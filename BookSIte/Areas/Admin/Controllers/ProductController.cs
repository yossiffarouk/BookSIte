using BookSite.DataAccess.Repository.Category;
using BookSite.DataAccess.Repository.Unitofwork;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Category = BookStore.Models.Category;




namespace BookSIte.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _Unit;

        public ProductController(IUnitOfWork Unit)
        {
            _Unit = Unit;

        }


        public IActionResult Index()
        {
            var Products = _Unit.ProductRepo.GetAll();
            return View(Products);
        }


        public IActionResult Create()
        {
            ProductVM ProductVM = new()
            {
                Category = _Unit.CategoryRepo.GetAll().Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.Id.ToString()
                }),
                Product = new Product(),
            };

            return View(ProductVM);
        }
        [HttpPost]
        public IActionResult Create(ProductVM ProductVM)
        {
            if (ModelState.IsValid)
            {
                _Unit.ProductRepo.Add(ProductVM.Product);
                _Unit.savechanges();
                TempData["Create"] = "New Product Added";
                return RedirectToAction("Index");

            }
            ProductVM.Category =  _Unit.CategoryRepo.GetAll().Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.Id.ToString()
                });

            return View(ProductVM);
        }

        public IActionResult Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Product = _Unit.ProductRepo.Get(a => a.Id == id);
            return View(Product);
        }
        [HttpPost]
        public IActionResult Edit(Product Product)
        {

            if (ModelState.IsValid)
            {
                _Unit.ProductRepo.Update(Product);
                _Unit.savechanges();
                TempData["Update"] = $"Product Has Updateded";
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
            var Product = _Unit.ProductRepo.Get(a => a.Id == id);
            TempData["Delete"] = $"{Product.Title} Has Deleteded";
            _Unit.ProductRepo.Remove(Product);
            _Unit.savechanges();
            return RedirectToAction("Index");
        }
    }
}
