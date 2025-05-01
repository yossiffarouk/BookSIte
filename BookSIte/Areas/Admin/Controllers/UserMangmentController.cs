using BookkStore.Utility;
using BookSite.DataAccess.Repository.Unitofwork;
using BookSIte.Data;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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

        public UserMangmentController(Context Context)
        {
            _context = Context;
        }
        public IActionResult Index()
        {
            return View();
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
