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
        private readonly Context _context;
        private readonly UserManager<IdentityUser> _userMangment;

        public UserMangmentController(Context Context, UserManager<IdentityUser> userMangment)
        {
            _context = Context;
            _userMangment = userMangment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleMangment(string userId)
        {
            string RoleID = _context.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;
            UserMangmentVM vm = new UserMangmentVM()
            {
                ApplicationsUser = _context.ApplicationsUsers.Include(u => u.Company).FirstOrDefault(a => a.Id == userId),
                Companys = _context.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),

                }),

                Roles = _context.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name,

                }),


            };

            vm.ApplicationsUser.Role = _context.Roles.FirstOrDefault(a => a.Id == RoleID).Name;

            return View(vm);
        }


        [HttpPost]
        public IActionResult RoleMangment(UserMangmentVM UserMangmentVM)
        {

            string RoleID = _context.UserRoles.FirstOrDefault(u => u.UserId == UserMangmentVM.ApplicationsUser.Id).RoleId;
            string oldRole = _context.Roles.FirstOrDefault(u => u.Id == RoleID).Name;

            if (!(UserMangmentVM.ApplicationsUser.Role == oldRole))
            {
                //a role was updated
                ApplicationsUser applicationUser = _context.ApplicationsUsers.FirstOrDefault(u => u.Id == UserMangmentVM.ApplicationsUser.Id);
                if (UserMangmentVM.ApplicationsUser.Role == SD.Role_User_Company)
                {
                    applicationUser.CompanyId = UserMangmentVM.ApplicationsUser.CompanyId;
                }
                if (oldRole == SD.Role_User_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _context.SaveChanges();

                _userMangment.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userMangment.AddToRoleAsync(applicationUser, UserMangmentVM.ApplicationsUser.Role).GetAwaiter().GetResult();

            }

            return RedirectToAction("Index");
        }

        public IActionResult getall()
        {
            var Users = _context.ApplicationsUsers.Include(a=>a.Company).ToList();

            var Roles = _context.Roles.ToList();
            var UserRoles = _context.UserRoles.ToList();

            foreach (var User in Users) {
                var roleId = UserRoles.FirstOrDefault(a=>a.UserId == User.Id).RoleId;
                User.Role = Roles.FirstOrDefault(a=>a.Id == roleId).Name;


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
            var UserFromDb = _context.ApplicationsUsers.FirstOrDefault(a=>a.Id == id);
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



            _context.SaveChanges();
            return Json(new { success = true, message = massege });
        }
    }
}
