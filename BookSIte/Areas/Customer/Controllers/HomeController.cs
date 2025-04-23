using BookSite.DataAccess.Repository.Unitofwork;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            var products = _unit.ProductRepo.GetAll(includeproperty: "Category");
            return View(products);
        }
        public IActionResult Details(int id) 
        {
            ShoppigCart shoppigCart = new ShoppigCart()
            {
                Product = _unit.ProductRepo.Get(a => a.Id == id, includeproperty: "Category"),
                Count = 1,
                ProductId = id
            };
        
            
            return View(shoppigCart);
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
