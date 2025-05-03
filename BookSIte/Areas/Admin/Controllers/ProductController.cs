using BookkStore.Utility;

using BookSite.DataAccess.Repository.Unitofwork;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;
using Category = BookStore.Models.Category;




namespace BookSIte.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_User_Admin)]
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

            ProductVM.Product = _Unit.ProductRepo.Get(a => a.Id == id , "ProductImages");
            return View(ProductVM);

        }
        [HttpPost]
        public IActionResult Upsert(ProductVM ProductVM,List<IFormFile>? files)
        {

            if (ModelState.IsValid)
            {


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

                string wwwRootPath = _WebHostEnvironment.WebRootPath;
                if (files!= null)
                {



                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + ProductVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            imageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = ProductVM.Product.Id,
                        };

                        if (ProductVM.Product.ProductImages == null)
                            ProductVM.Product.ProductImages = new List<ProductImage>();

                        ProductVM.Product.ProductImages.Add(productImage);

                    }
                }
                _Unit.ProductRepo.Update(ProductVM.Product);
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
     
  
        public IActionResult DeleteImage(int imageId)
        {
            var Imagetodelete = _Unit.ProductImageRepo.Get(a => a.Id == imageId);

            var proudectId = _Unit.ProductRepo.Get(a => a.Id == Imagetodelete.ProductId);
            if (Imagetodelete != null)
            {
                var oldimagepath = Path.Combine(_WebHostEnvironment.WebRootPath, Imagetodelete.imageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldimagepath))
                {
                    System.IO.File.Delete(oldimagepath);
                }

                _Unit.ProductImageRepo.Remove(Imagetodelete);
                _Unit.savechanges();
                TempData["Delete"] = " Image Has Deleteded";
            }






            return RedirectToAction(nameof(Upsert), new { id = proudectId.Id });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var proudecttodelete = _Unit.ProductRepo.Get(a=>a.Id == id);
            if (proudecttodelete == null)
            {
                return Json(new { success = false , message = "erorrrrr"});
            }


            //var oldimagepath = Path.Combine(_WebHostEnvironment.WebRootPath, proudecttodelete.ImageUrl.TrimStart('\\'));

            //if (System.IO.File.Exists(oldimagepath))
            //{
            //    System.IO.File.Delete(oldimagepath);
            //}
           
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
