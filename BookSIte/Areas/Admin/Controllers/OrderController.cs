using BookkStore.Utility;
using BookSite.DataAccess.Repository.Unitofwork;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace BookSIte.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _Unit;

        [BindProperty]
        public OrderVM orderVM { get; set; }


        public OrderController(IUnitOfWork Unit)
        {
            _Unit = Unit;

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderid)
        {
            orderVM = new()
            {
                Header = _Unit.OrderHeaderRepo.Get(a => a.Id == orderid, includeproperty: "ApplicationUser"),
                OrderDetailList = _Unit.OrderDetailRepo.GetAll(includeproperty: "Product", a => a.OrderHeaderId == orderid)
            };
            return View(orderVM);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Employee)]
        public IActionResult UpdateOrderDetail(int orderid)
        {
            var orderfromdb = _Unit.OrderHeaderRepo.Get(a => a.Id == orderid);
            orderfromdb.Name = orderVM.Header.Name;
            orderfromdb.PhoneNumber = orderVM.Header.PhoneNumber;
            orderfromdb.StreetAddress = orderVM.Header.StreetAddress;
            orderfromdb.City = orderVM.Header.City;
            orderfromdb.PostalCode = orderVM.Header.PostalCode;
            if (!string.IsNullOrEmpty(orderfromdb.Carrier))
            {
                orderfromdb.Carrier = orderVM.Header.Carrier;
            }
            if (!string.IsNullOrEmpty(orderfromdb.TrackingNumber))
            {
                orderfromdb.TrackingNumber = orderVM.Header.TrackingNumber;
            }
            _Unit.OrderHeaderRepo.Update(orderfromdb);
            _Unit.savechanges();


            TempData["Update"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), new { orderid = orderfromdb.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Employee)]
        public IActionResult StartProcessing()
        {
            _Unit.OrderHeaderRepo.UpdateStatus(orderVM.Header.Id, SD.StatusInProcess);
            _Unit.savechanges();


            TempData["Update"] = "Order Updated In InProcess Mode";
            return RedirectToAction(nameof(Details), new { orderid = orderVM.Header.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Employee)]

        public IActionResult ShipOrder()
        {

            var orderHeader = _Unit.OrderHeaderRepo.Get(u => u.Id == orderVM.Header.Id);
            orderHeader.TrackingNumber = orderVM.Header.TrackingNumber;
            orderHeader.Carrier = orderVM.Header.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _Unit.OrderHeaderRepo.Update(orderHeader);
            _Unit.savechanges();
            TempData["Update"] = "Order Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.Header.Id });
        }

        public IActionResult CancelOrder()
        {

            var orderHeader = _Unit.OrderHeaderRepo.Get(u => u.Id == orderVM.Header.Id);

            // when add payment get way this line to refund money to clinet
            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _Unit.OrderHeaderRepo.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _Unit.OrderHeaderRepo.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _Unit.savechanges();

            TempData["Delete"] = $"Order with id : {orderVM.Header.Id} Was  Cancelled";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.Header.Id });
        }


        [ActionName("Details")]
        [HttpPost]
        public IActionResult Details_PAY_NOW()
        {
            orderVM.Header = _Unit.OrderHeaderRepo
                .Get(u => u.Id == orderVM.Header.Id, includeproperty: "ApplicationUser");
            orderVM.OrderDetailList = _Unit.OrderDetailRepo
                .GetAll(includeproperty: "Product", u => u.OrderHeaderId == orderVM.Header.Id);

            //it is a regular customer account and we need to capture payment
            //stripe logic
            var domain = Request.Scheme + "//" + Request.Host.Value + "/" ;
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Admin/Order/PaymentConfirmation?orderHeaderId={orderVM.Header.Id}",
                CancelUrl = domain + $"Admin/Order/details?orderId={orderVM.Header.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in orderVM.OrderDetailList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
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
            _Unit.OrderHeaderRepo.UpdateStripePaymentID(orderVM.Header.Id, session.Id, session.PaymentIntentId);
            _Unit.savechanges();
            
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }


        //PaymentConfirmation
        public IActionResult PaymentConfirmation(int orderHeaderId)
        {

            OrderHeader orderHeader = _Unit.OrderHeaderRepo.Get(u => u.Id == orderHeaderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                //this is an order by company

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _Unit.OrderHeaderRepo.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
                    _Unit.OrderHeaderRepo.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _Unit.savechanges();
                }


            }


            return View(orderHeaderId);
        }





        public IActionResult getall(string status)
        {
            var Orders = _Unit.OrderHeaderRepo.GetAll(includeproperty: "ApplicationUser");



            if (!User.IsInRole(SD.Role_User_Admin))
            {
                var ClaimIdentity = (ClaimsIdentity)User.Identity;
                var userId = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                Orders = _Unit.OrderHeaderRepo.GetAll(includeproperty: "ApplicationUser" , a=>a.ApplicationUserId == userId);
            }

            switch (status)
            {
                case "pending":
                    Orders = _Unit.OrderHeaderRepo.GetAll(includeproperty: "ApplicationUser", a => a.PaymentStatus == SD.PaymentStatusDelayedPayment);
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
