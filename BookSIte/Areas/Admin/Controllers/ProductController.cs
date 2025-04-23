using BookkStore.Utility;
using BookSite.DataAccess.Repository.Category;
using BookSite.DataAccess.Repository.Unitofwork;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Category = BookStore.Models.Category;




namespace BookSIte.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_User_Admin)]
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


        public IActionResult Upsert(int? id )
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
            if (id ==null || id == 0)
            {
                return View(ProductVM);

            }

            ProductVM.Product = _Unit.ProductRepo.Get(a => a.Id == id);
            return View(ProductVM);

        }
        [HttpPost]
        public IActionResult Upsert(ProductVM ProductVM, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwrootpath = _WebHostEnvironment.WebRootPath;
                if (file != null)
                {


                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
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

                    ProductVM.Product.ImageUrl = @"\images\product\" + filename;
                }



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
            ProductVM.Category = _Unit.CategoryRepo.GetAll().Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.Id.ToString()
            });

            return View(ProductVM);
        }



        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var proudecttodelete = _Unit.ProductRepo.Get(a=>a.Id == id);
            if (proudecttodelete == null)
            {
                return Json(new { success = false , message = "erorrrrr"});
            }


            var oldimagepath = Path.Combine(_WebHostEnvironment.WebRootPath, proudecttodelete.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldimagepath))
            {
                System.IO.File.Delete(oldimagepath);
            }
           
            TempData["Delete"] = $"{proudecttodelete.Title} Has Deleteded";
            _Unit.ProductRepo.Remove(proudecttodelete);
            _Unit.savechanges();
            return Json(new { success = true, message = TempData["Delete"] });
        }





        public IActionResult getall()
        {
            var Products = _Unit.ProductRepo.GetAll(includeproperty: "Category");
            return Json(new {data = Products });
        }




    }
}
