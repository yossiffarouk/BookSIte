using BookSite.DataAccess.Repository.Category;
using BookSite.DataAccess.Repository.Unitofwork;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Category = BookStore.Models.Category;




namespace BookSIte.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _Unit;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public ProductController(IUnitOfWork Unit, IWebHostEnvironment webHostEnvironment)
        {
            _Unit = Unit;
            _WebHostEnvironment = webHostEnvironment;
        }


        public IActionResult Index()
        {
            var Products = _Unit.ProductRepo.GetAll(includeproperty : "Category");
            return View(Products);
        }


        public IActionResult Upsert()
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
        public IActionResult Upsert(ProductVM ProductVM , IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwrootpath = _WebHostEnvironment.WebRootPath;
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(wwwrootpath);
                string productPath = Path.Combine(wwwrootpath, @"images\product");

                if (!string.IsNullOrEmpty(ProductVM.Product.ImageUrl))
                {
                    var oldimagepath = Path.Combine(wwwrootpath, ProductVM.Product.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldimagepath))
                    {
                        System.IO.File.Delete(oldimagepath);
                    }
                }
                using (var filestrme = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                {
                    file.CopyTo(filestrme);
                }

                ProductVM.Product.ImageUrl = @"\images\product" + filename;



                if (ProductVM.Product.Id == 0)
                {
                _Unit.ProductRepo.Add(ProductVM.Product);
                TempData["Create"] = "New Product Added";

                }
                else
                {
                    _Unit.ProductRepo.Update(ProductVM.Product);
                    TempData["Update"] = $"Product Has Updateded";
                }
                _Unit.savechanges();
                return RedirectToAction("Index");

            }
            ProductVM.Category =  _Unit.CategoryRepo.GetAll().Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.Id.ToString()
                });

            return View(ProductVM);
        }

        //public IActionResult Edit(int id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    var Product = _Unit.ProductRepo.Get(a => a.Id == id);
        //    return View(Product);
        //}
        //[HttpPost]
        //public IActionResult Edit(Product Product)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        _Unit.ProductRepo.Update(Product);
        //        _Unit.savechanges();
        //        TempData["Update"] = $"Product Has Updateded";
        //        return RedirectToAction("Index");

        //    }
        //    return View();
        //}


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
