using BookkStore.Utility;
using BookSite.DataAccess.Repository.Unitofwork;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookSIte.ViewCoponent
{
    public class ShoppingCartViewComponent : ViewComponent
    {

        private readonly IUnitOfWork _unit;

        public ShoppingCartViewComponent(IUnitOfWork unit)
        {
            _unit = unit;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var claims = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims != null)
            {
                if ( HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart, _unit.ShoppinCartRepo.GetAll("", a => a.ApplicationsUserId == claims.Value).Count());
                    
                }

                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }

    }
}
