using BookkStore.Utility;
using BookSite.DataAccess.Repository.Unitofwork;
using BookSIte.Data;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace BookSIte.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unit;
        
        [BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }
		public CartController(IUnitOfWork unit) 
        {
           
            _unit = unit;
           


		}
        [Authorize]
        public IActionResult Index()
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var userId = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new()
			{
				ShoppingCartsList = _unit.ShoppinCartRepo.GetAll("Product",u => u.ApplicationsUserId == userId),
				OrderHeaders = new()
			};

			foreach (var item in ShoppingCartVM.ShoppingCartsList)
            {
                item.ItemPrice = GetBriceBasedOnQuntity(item);
				ShoppingCartVM.OrderHeaders.OrderTotal += (item.ItemPrice * item.Count);

            }
            return View(ShoppingCartVM);
        }

        public IActionResult plus(int id)
        {
            var cartItem = _unit.ShoppinCartRepo.Get(a=>a.Id == id);
            cartItem.Count += 1;
            _unit.ShoppinCartRepo.Update(cartItem);
            _unit.savechanges();




           return RedirectToAction(nameof(Index));
        }

        public IActionResult minus(int id)
        {
            var cartItem = _unit.ShoppinCartRepo.Get(a => a.Id == id);
            if (cartItem.Count <= 1)
            {
                _unit.ShoppinCartRepo.Remove(cartItem);
            }
            else
            {

                 cartItem.Count -= 1;
                 _unit.ShoppinCartRepo.Update(cartItem);
            }

            _unit.savechanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var cartItem = _unit.ShoppinCartRepo.Get(a => a.Id == id);
           
            _unit.ShoppinCartRepo.Remove(cartItem);
            
            _unit.savechanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var userId = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
		
			ShoppingCartVM = new()
			{
				ShoppingCartsList = _unit.ShoppinCartRepo.GetAll("Product", u => u.ApplicationsUserId == userId),
				OrderHeaders = new()
			};

			ShoppingCartVM.OrderHeaders.ApplicationUser = _unit.ApplicationRepo.Get(u => u.Id == userId);

			ShoppingCartVM.OrderHeaders.Name = ShoppingCartVM.OrderHeaders.ApplicationUser.Name;
			ShoppingCartVM.OrderHeaders.PhoneNumber = ShoppingCartVM.OrderHeaders.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeaders.StreetAddress = ShoppingCartVM.OrderHeaders.ApplicationUser.StreetAddress;
			ShoppingCartVM.OrderHeaders.City = ShoppingCartVM.OrderHeaders.ApplicationUser.City;
			ShoppingCartVM.OrderHeaders.PostalCode = ShoppingCartVM.OrderHeaders.ApplicationUser.PostalCode;

            foreach (var item in ShoppingCartVM.ShoppingCartsList)    
            {
                item.ItemPrice = GetBriceBasedOnQuntity(item);
				ShoppingCartVM.OrderHeaders.OrderTotal += (item.ItemPrice * item.Count);

            }

            return View(ShoppingCartVM);
            
        }
        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPost()
		{
			var ClaimIdentity = (ClaimsIdentity)User.Identity;
			var userId = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            
            ShoppingCartVM.ShoppingCartsList = _unit.ShoppinCartRepo.GetAll("Product", u => u.ApplicationsUserId == userId);
			

			ShoppingCartVM.OrderHeaders.OrderDate = DateTime.Now;
			ShoppingCartVM.OrderHeaders.ApplicationUserId = userId;

            var appuser = _unit.ApplicationRepo.Get(a=>a.Id == userId);
			foreach (var item in ShoppingCartVM.ShoppingCartsList)
			{
				item.ItemPrice = GetBriceBasedOnQuntity(item);
				ShoppingCartVM.OrderHeaders.OrderTotal += (item.ItemPrice * item.Count);

			}


            if (ShoppingCartVM.OrderHeaders.ApplicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // no company 
                ShoppingCartVM.OrderHeaders.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeaders.OrderStatus = SD.PaymentStatusPending;


            }
            else
            {
                // has company
                ShoppingCartVM.OrderHeaders.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeaders.OrderStatus = SD.StatusApproved;
            }


            _unit.OrderHeaderRepo.Add(ShoppingCartVM.OrderHeaders);
            _unit.savechanges();


            foreach (var item in ShoppingCartVM.ShoppingCartsList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeaders.Id,
                    Price = item.ItemPrice,
                    Count = item.Count,
				};
				_unit.OrderDetailRepo.Add(orderDetail);
			}
			_unit.savechanges();

			return View(ShoppingCartVM);

		}





		private double GetBriceBasedOnQuntity(ShoppingCart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            }
            else
            {
                if (cart.Count <= 100)
                {
                    return cart.Product.Price50;
                }
                else
                {
                    return cart.Product.Price100;
                }
            }
        }
    }
}
