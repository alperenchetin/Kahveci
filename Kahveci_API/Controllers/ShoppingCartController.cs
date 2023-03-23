using Azure;
using Kahveci_API.Data;
using Kahveci_API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Kahveci_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly AppDbContext _db;
        protected ApiResponse _apiResponse;

        public ShoppingCartController(AppDbContext db)
        {
            _apiResponse = new ApiResponse();
            _db = db;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> Get(string userId)
        {
            try
            {
                ShoppingCart cart;

                if (string.IsNullOrEmpty(userId))
                {
                    cart = new();
                }
                else
                {
                    cart = _db.ShoppingCarts.Include(x => x.CartItems).ThenInclude(x => x.MenuItem).FirstOrDefault(x => x.UserId == userId);
                }

                //if (cart.CartItems != null && cart.CartItems.Count() > 0)
                //{
                //    cart.CartTotal = cart.CartItems.Sum(x => x.Quantity * x.MenuItem.Price);
                //}

                _apiResponse.Result = cart;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception e)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string>() { e.ToString() };
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
            }
            return _apiResponse;
        }
        //[HttpPost]
        //public async Task<ActionResult<ApiResponse>> AddUpdate(string userId, int menuItemId, int updateQuantity)//Guid de olabilir
        //{
        //    ShoppingCart cart = _db.ShoppingCarts.Include(x => x.CartItems).FirstOrDefault(x => x.UserId == userId);
        //    MenuItem menuItem = _db.MenuItems.FirstOrDefault(x => x.Id == menuItemId);
        //    if (menuItem == null)
        //    {
        //        _apiResponse.StatusCode = HttpStatusCode.BadRequest;
        //        _apiResponse.IsSuccess = false;
        //        return BadRequest(_apiResponse);
        //    }
        //    if (cart == null && updateQuantity > 0)
        //    {
        //        ShoppingCart newCart = new() { UserId = userId };
        //        _db.ShoppingCarts.Add(newCart);
        //        _db.SaveChanges();

        //        CartItem newCartItem = new()
        //        {
        //            MenuItemId = menuItemId,
        //            Quantity = updateQuantity,
        //            ShoppingCartId = newCart.Id,
        //            MenuItem = null,
        //        };
        //        _db.CartItems.Add(newCartItem);
        //        _db.SaveChanges();
        //    }
        //    else
        //    {
        //        CartItem cartItemExisting = cart.CartItems.FirstOrDefault(x => x.MenuItemId == menuItemId);
        //        if (cartItemExisting == null)
        //        {
        //            CartItem newCartItem = new()
        //            {
        //                MenuItemId = menuItemId,
        //                Quantity = updateQuantity,
        //                ShoppingCartId = cart.Id,
        //                MenuItem = null,
        //            };
        //            _db.CartItems.Add(newCartItem);
        //            _db.SaveChanges();
        //        }
        //        else
        //        {
        //            int newQuantity = cartItemExisting.Quantity + updateQuantity;
        //            if (updateQuantity == 0 || newQuantity <= 0)
        //            {
        //                _db.CartItems.Remove(cartItemExisting);
        //                if (cart.CartItems.Count() == 1)
        //                {
        //                    _db.ShoppingCarts.Remove(cart);
        //                }
        //                _db.SaveChanges();
        //            }
        //            else
        //            {
        //                cartItemExisting.Quantity=cartItemExisting.Quantity+updateQuantity;
        //                _db.SaveChanges();
        //            }
        //        }
        //    }
        //    return _apiResponse;
        //}
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> AddOrUpdateItemInCart(string userId, int menuItemId, int updateQuantityBy)
        {

            ShoppingCart shoppingCart = _db.ShoppingCarts.Include(u => u.CartItems).FirstOrDefault(u => u.UserId == userId);
            MenuItem menuItem = _db.MenuItems.FirstOrDefault(u => u.Id == menuItemId);
            if (menuItem == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                return BadRequest(_apiResponse);
            }
            if (shoppingCart == null && updateQuantityBy > 0)
            {
                //create a shopping cart & add cart item

                ShoppingCart newCart = new() { UserId = userId };
                _db.ShoppingCarts.Add(newCart);
                _db.SaveChanges();

                CartItem newCartItem = new()
                {
                    MenuItemId = menuItemId,
                    Quantity = updateQuantityBy,
                    ShoppingCartId = newCart.Id,
                    MenuItem = null
                };
                _db.CartItems.Add(newCartItem);
                _db.SaveChanges();
            }
            else
            {
                //shopping cart exists

                CartItem cartItemInCart = shoppingCart.CartItems.FirstOrDefault(u => u.MenuItemId == menuItemId);
                if (cartItemInCart == null)
                {
                    //item does not exist in current cart
                    CartItem newCartItem = new()
                    {
                        MenuItemId = menuItemId,
                        Quantity = updateQuantityBy,
                        ShoppingCartId = shoppingCart.Id,
                        MenuItem = null
                    };
                    _db.CartItems.Add(newCartItem);
                    _db.SaveChanges();
                }
                else
                {
                    //item already exist in the cart and we have to update quantity
                    int newQuantity = cartItemInCart.Quantity + updateQuantityBy;
                    if (updateQuantityBy == 0 || newQuantity <= 0)
                    {
                        //remove cart item from cart and if it is the only item then remove cart
                        _db.CartItems.Remove(cartItemInCart);
                        if (shoppingCart.CartItems.Count() == 1)
                        {
                            _db.ShoppingCarts.Remove(shoppingCart);
                        }
                        _db.SaveChanges();
                    }
                    else
                    {
                        cartItemInCart.Quantity = newQuantity;
                        _db.SaveChanges();
                    }
                }
            }
            return _apiResponse;

        }
    }
}
