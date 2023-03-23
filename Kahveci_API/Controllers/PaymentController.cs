using Azure;
using Kahveci_API.Data;
using Kahveci_API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Net;

namespace Kahveci_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;
        protected ApiResponse _apiResponse;

        public PaymentController(IConfiguration config, AppDbContext db)
        {
            _config = config;
            _apiResponse = new ApiResponse();
            _db = db;
        }
        //    [HttpPost]
        //    public async Task<ActionResult<ApiResponse>> Payment(string userId)
        //    {
        //        ShoppingCart cart = _db.ShoppingCarts.Include(x => x.CartItems).ThenInclude(x => x.MenuItem).FirstOrDefault(x => x.UserId == userId);
        //        if (cart == null || cart.CartItems == null || cart.CartItems.Count() == 0)
        //        {
        //            _apiResponse.StatusCode = HttpStatusCode.BadRequest;
        //            _apiResponse.IsSuccess = false;
        //            return BadRequest();
        //        }
        //        #region Create Payment Intent
        //        StripeConfiguration.ApiKey = _config["StripeSettings:Key"];
        //        cart.CartTotal = cart.CartItems.Sum(x => x.Quantity * x.MenuItem.Price);

        //        var options = new PaymentIntentCreateOptions
        //        {
        //            Amount = Convert.ToInt32(cart.CartTotal*100),
        //            Currency = "tl",
        //            PaymentMethodTypes = new List<string> { "card" },
        //        };
        //        var service = new PaymentIntentService();
        //        PaymentIntent response = service.Create(options);
        //        cart.StripePaymentIntentId = response.Id;
        //        cart.ClientSecret = response.ClientSecret;

        //        #endregion
        //        _apiResponse.Result = cart;
        //        _apiResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(_apiResponse);
        //    }
        //}
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> MakePayment(string userId)
        {
            ShoppingCart shoppingCart = _db.ShoppingCarts
                .Include(u => u.CartItems)
                .ThenInclude(u => u.MenuItem).FirstOrDefault(u => u.UserId == userId);

            if (shoppingCart == null || shoppingCart.CartItems == null || shoppingCart.CartItems.Count() == 0)
            {
                _apiResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                return BadRequest(_apiResponse);
            }

            #region Create Payment Intent

            StripeConfiguration.ApiKey = _config["StripeSettings:Key"];
            shoppingCart.CartTotal = shoppingCart.CartItems.Sum(u => u.Quantity * u.MenuItem.Price);

            PaymentIntentCreateOptions options = new()
            {
                Amount = (int)(shoppingCart.CartTotal * 100),
                Currency = "usd",
                PaymentMethodTypes = new List<string>
                  {
                    "card",
                  },
            };
            PaymentIntentService service = new();
            PaymentIntent response = service.Create(options);
            shoppingCart.StripePaymentIntentId = response.Id;
            shoppingCart.ClientSecret = response.ClientSecret;


            #endregion

            _apiResponse.Result = shoppingCart;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

    }
}
