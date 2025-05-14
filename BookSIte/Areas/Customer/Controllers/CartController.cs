using BookkStore.Utility;
using BookSite.DataAccess.Repository.Unitofwork;
using BookSIte.Data;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookSIte.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]

    public class CartController : Controller
    {
        private readonly IUnitOfWork _unit;
        
        [BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }
		public CartController(IUnitOfWork unit) 
        {
           
            _unit = unit;
           


		}
        public IActionResult Index()
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var userId = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new()
			{
				ShoppingCartsList = _unit.ShoppinCartRepo.GetAll("Product",u => u.ApplicationsUserId == userId),
				OrderHeaders = new()
			};

            var ProductImageList = _unit.ProductImageRepo.GetAll();


            foreach (var item in ShoppingCartVM.ShoppingCartsList)
            {
                item.Product.ProductImages = ProductImageList.Where(u => u.ProductId == item.Product.Id).ToList();
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
                HttpContext.Session.SetInt32(SD.SessionCart, _unit.ShoppinCartRepo.GetAll("", a => a.ApplicationsUserId == cartItem.ApplicationsUserId).Count() - 1);
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
             HttpContext.Session.SetInt32(SD.SessionCart, _unit.ShoppinCartRepo.GetAll("", a => a.ApplicationsUserId == cartItem.ApplicationsUserId).Count() - 1);

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

            if (ShoppingCartVM.ShoppingCartsList.Count() == 0)
            {
                return RedirectToAction(nameof(Index));
            }

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

            var appuser = _unit.ApplicationRepo.Get(a => a.Id == userId);
            foreach (var item in ShoppingCartVM.ShoppingCartsList)
			{
				item.ItemPrice = GetBriceBasedOnQuntity(item);
				ShoppingCartVM.OrderHeaders.OrderTotal += (item.ItemPrice * item.Count);

			}


            if (appuser.CompanyId.GetValueOrDefault() == 0)
            {
                // no company 
                ShoppingCartVM.OrderHeaders.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeaders.OrderStatus = SD.StatusPending;


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





            if (appuser.CompanyId.GetValueOrDefault() == 0)
            {

                //it is a regular customer account and we need to capture payment
                //stripe logic
                var domain = Request.Scheme + "//" + Request.Host.Value + "/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeaders.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartVM.ShoppingCartsList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.ItemPrice * 100), // $20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }


                var service = new SessionService();
                Session session = service.Create(options);
                _unit.OrderHeaderRepo.UpdateStripePaymentID(ShoppingCartVM.OrderHeaders.Id, session.Id, session.PaymentIntentId);
                _unit.savechanges();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);

            }





            return RedirectToAction(nameof (OrderConfirmation) , new  {id = ShoppingCartVM.OrderHeaders.Id });

		}
        public IActionResult OrderConfirmation(int id)
        {


            OrderHeader orderHeader = _unit.OrderHeaderRepo.Get(u => u.Id == id, "ApplicationUser");



            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //this is an order by customer

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unit.OrderHeaderRepo.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _unit.OrderHeaderRepo.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unit.savechanges();
                }


            }

            List<ShoppingCart> shoppingCarts = _unit.ShoppinCartRepo
             .GetAll("",u => u.ApplicationsUserId == orderHeader.ApplicationUserId).ToList();
            _unit.ShoppinCartRepo.RemoveRange(shoppingCarts);
            _unit.savechanges();

                
            HttpContext.Session.Clear();
            return View(id);

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
