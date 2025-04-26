using BookkStore.Utility;

using BookSite.DataAccess.Repository.Unitofwork;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.InteropServices;
using Category = BookStore.Models.Category;




namespace BookSIte.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_User_Admin)]
    public class CompanyController : Controller
    {

        private readonly IUnitOfWork _Unit;
       

        public CompanyController(IUnitOfWork Unit)
        {
            _Unit = Unit;
            
        }


        public IActionResult Index()
        {
            var Companys = _Unit.CompanyRepo.GetAll();
            return View(Companys);
        }


        public IActionResult Upsert(int? id )
        {
        
            if (id ==null || id == 0)
            {
                return View(new Company());

            }

            var Company = _Unit.CompanyRepo.Get(a => a.Id == id);
            return View(Company);

        }
        [HttpPost]
        public IActionResult Upsert(Company Company)
        {

            if (ModelState.IsValid)
            {
                



                if (Company.Id == 0)
                {
                    _Unit.CompanyRepo.Add(Company);
                    TempData["Create"] = "New Company Added";

                }
                else
                {
                    _Unit.CompanyRepo.Update(Company);
                    TempData["Update"] = $"Company Has Updateded";
                }
                _Unit.savechanges();
                return RedirectToAction("Index");

            }
            var Category = _Unit.CategoryRepo.GetAll();
            

            return View(Category);
        }



        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var Companytodelete = _Unit.CompanyRepo.Get(a=>a.Id == id);
            if (Companytodelete == null)
            {
                return Json(new { success = false , message = "erorrrrr"});
            }


           
           
            TempData["Delete"] = $"{Companytodelete.Name} Has Deleteded";
            _Unit.CompanyRepo.Remove(Companytodelete);
            _Unit.savechanges();
            return Json(new { success = true, message = TempData["Delete"] });
        }





        public IActionResult getall()
        {
            var Companys = _Unit.CompanyRepo.GetAll();
            return Json(new {data = Companys });
        }




    }
}
