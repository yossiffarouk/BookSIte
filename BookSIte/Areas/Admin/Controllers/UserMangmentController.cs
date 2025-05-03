using BookkStore.Utility;
using BookSite.DataAccess.Repository.Unitofwork;
using BookSIte.Data;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Security.Claims;

namespace BookSIte.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UserMangmentController : Controller
    {
        private readonly IUnitOfWork _Unit;
        private readonly UserManager<IdentityUser> _userMangment;
        private readonly RoleManager<IdentityRole> _roleMangment;

        public UserMangmentController( UserManager<IdentityUser> userMangment,  IUnitOfWork Unit , RoleManager<IdentityRole> roleMangment)
        {
             _Unit = Unit;
             _userMangment = userMangment;
             _roleMangment = roleMangment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleMangment(string userId)
        {
            
            UserMangmentVM vm = new UserMangmentVM()
            {
                ApplicationsUser = _Unit.ApplicationRepo.Get(a => a.Id == userId , "Company"),
                Companys = _Unit.CompanyRepo.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),

                }),

                Roles = _roleMangment.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),


            };

            vm.ApplicationsUser.Role = _userMangment.GetRolesAsync(_Unit.ApplicationRepo.Get(a => a.Id == userId, "Company"))
                                                                                        .GetAwaiter()
                                                                                        .GetResult()
                                                                                        .FirstOrDefault();

            return View(vm);
        }


        [HttpPost]
        public IActionResult RoleMangment(UserMangmentVM UserMangmentVM)
        {


            string oldRole = _userMangment.GetRolesAsync(_Unit.ApplicationRepo.Get(a => a.Id == UserMangmentVM.ApplicationsUser.Id, "Company"))
                                                                                        .GetAwaiter()
                                                                                        .GetResult()
                                                                                        .FirstOrDefault();

            ApplicationsUser applicationUser = _Unit.ApplicationRepo.Get(u => u.Id == UserMangmentVM.ApplicationsUser.Id);
            if (!(UserMangmentVM.ApplicationsUser.Role == oldRole))
            {
                //a role was updated
                if (UserMangmentVM.ApplicationsUser.Role == SD.Role_User_Company)
                {
                    applicationUser.CompanyId = UserMangmentVM.ApplicationsUser.CompanyId;
                }
                if (oldRole == SD.Role_User_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _Unit.ApplicationRepo.Update(applicationUser);
                _Unit.savechanges();

                _userMangment.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userMangment.AddToRoleAsync(applicationUser, UserMangmentVM.ApplicationsUser.Role).GetAwaiter().GetResult();

            }
            else
            {
                if (oldRole == SD.Role_User_Company && applicationUser.CompanyId != UserMangmentVM.ApplicationsUser.CompanyId)
                {
                    applicationUser.CompanyId = UserMangmentVM.ApplicationsUser.CompanyId;
                    _Unit.ApplicationRepo.Update(applicationUser);
                    _Unit.savechanges();
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult getall()
        {
            var Users = _Unit.ApplicationRepo.GetAll("Company");

         

            foreach (var User in Users) {

                User.Role = _userMangment.GetRolesAsync(User)
                                            .GetAwaiter()
                                            .GetResult()
                                            .FirstOrDefault();


                if (User.Company == null)
                {
                    User.Company = new Company();
                    User.Company.Name = "None";
                }
            }


           
            return Json(new { data = Users });
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var UserFromDb = _Unit.ApplicationRepo.Get(a=>a.Id == id);
            if (UserFromDb == null)
            {
                return Json(new { success = true, message = "Error while Locking/Unlocking" });
            }
            var massege = "";
            if (UserFromDb.LockoutEnd != null && UserFromDb.LockoutEnd > DateTime.Now)
            {
                UserFromDb.LockoutEnd = DateTime.Now;
                massege= $"{UserFromDb.Name} Has Unlocked";
            }
            else
            {
                UserFromDb.LockoutEnd = DateTime.Now.AddDays(3);
                massege = $"{UserFromDb.Name} Has locked";
            }



            _Unit.savechanges();
            return Json(new { success = true, message = massege });
        }
    }
}
