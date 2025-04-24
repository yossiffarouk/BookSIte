using BookSite.DataAccess.Repository.Unitofwork;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookSIte.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unit;


        public CartController(IUnitOfWork unit)
        {
           
            _unit = unit;

        }
        [Authorize]
        public IActionResult Index()
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var userId = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartsList = _unit.ShoppinCartRepo.GetAll( "Product" ,a =>a.ApplicationsUserId == userId ),
            };

            foreach (var item in shoppingCartVM.ShoppingCartsList)
            {
                item.ItemPrice = GetBriceBasedOnQuntity(item);
                shoppingCartVM.TotalOrder += (item.ItemPrice * item.Count);

            }
            return View(shoppingCartVM);
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
            return View();
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
