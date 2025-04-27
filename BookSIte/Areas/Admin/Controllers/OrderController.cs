using BookkStore.Utility;
using BookSite.DataAccess.Repository.Unitofwork;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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




		public IActionResult getall(string status)
		{
			var Orders = _Unit.OrderHeaderRepo.GetAll(includeproperty: "ApplicationUser");


            switch (status)
            {
                case "pending":
                    Orders = _Unit.OrderHeaderRepo.GetAll(includeproperty: "ApplicationUser" , a=>a.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    Orders = _Unit.OrderHeaderRepo.GetAll(includeproperty: "ApplicationUser", a => a.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    Orders = _Unit.OrderHeaderRepo.GetAll(includeproperty: "ApplicationUser", a => a.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    Orders = _Unit.OrderHeaderRepo.GetAll(includeproperty: "ApplicationUser", a => a.OrderStatus == SD.StatusApproved);
                    break;

                default:
                    break;
            }

            return Json(new { data = Orders });
		}
	}
}
