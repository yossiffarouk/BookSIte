using BookkStore.Utility;
using BookSite.DataAccess.Repository.Unitofwork;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookSIte.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unit;


        public HomeController(ILogger<HomeController> logger , IUnitOfWork unit)
        {
            _logger = logger;

            _unit = unit;

        }

        public IActionResult Index()
        {
           
            var products = _unit.ProductRepo.GetAll(includeproperty: "Category,ProductImages");
            return View(products);
        }
        public IActionResult Details(int id) 
        {
			ShoppingCart cart = new()
			{
				Product = _unit.ProductRepo.Get( u => u.Id == id , "Category,ProductImages"),
				Count = 1,
				ProductId = id
			};


			return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppigCart)
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var userId = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppigCart.ApplicationsUserId = userId;

            // there error here cheack it , if i remove this line it will lead to cant savechange in db cause of id
            shoppigCart.Id = 0;


            var cartfromdb = _unit.ShoppinCartRepo.Get(a=>a.ApplicationsUserId == userId && a.ProductId == shoppigCart.ProductId);
            if (cartfromdb != null)
            {
                cartfromdb.Count += shoppigCart.Count;
                _unit.ShoppinCartRepo.Update(cartfromdb);
                _unit.savechanges();
            }
            else
            {
                _unit.ShoppinCartRepo.Add(shoppigCart);
                _unit.savechanges();   
                HttpContext.Session.SetInt32(SD.SessionCart, _unit.ShoppinCartRepo.GetAll("" ,a => a.ApplicationsUserId == userId).Count());

            }



            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
