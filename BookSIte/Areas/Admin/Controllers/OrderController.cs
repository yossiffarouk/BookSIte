using BookSite.DataAccess.Repository.Unitofwork;
using Microsoft.AspNetCore.Mvc;

namespace BookSIte.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _Unit;
		

		public OrderController(IUnitOfWork Unit)
		{
			_Unit = Unit;
		
		}
		public IActionResult Index()
		{
			return View();
		}




		public IActionResult getall()
		{
			var Orders = _Unit.OrderHeaderRepo.GetAll(includeproperty: "ApplicationUser");
			
			return Json(new { data = Orders });
		}
	}
}
