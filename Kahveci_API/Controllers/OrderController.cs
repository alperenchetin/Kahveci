using Azure;
using Kahveci_API.Data;
using Kahveci_API.Dto;
using Kahveci_API.Entities;
using Kahveci_API.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Kahveci_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _db;
        protected ApiResponse _apiResponse;

        public OrderController(AppDbContext db)
        {
            _apiResponse = new ApiResponse();
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetOrders(string? userId)
        {
            try
            {
                var orderHeaders = _db.OrderHeaders.Include(x => x.OrderDetail).ThenInclude(x => x.MenuItem).OrderByDescending(x => x.OrderHeaderId);
                if (!string.IsNullOrEmpty(userId))
                {
                    _apiResponse.Result = orderHeaders.Where(x => x.AppUserId == userId);
                }
                else
                {
                    _apiResponse.Result = orderHeaders;
                }
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception e)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string>() { e.ToString() };
            }
            return _apiResponse;
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse>> GetOrders(int id)
        {
            try
            {
                if (id == 0)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                var orderHeaders = _db.OrderHeaders.Include(x => x.OrderDetail).ThenInclude(x => x.MenuItem).Where(x => x.OrderHeaderId == id);
                if (orderHeaders == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                _apiResponse.Result = orderHeaders;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);

            }
            catch (Exception e)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string>() { e.ToString() };
            }
            return _apiResponse;
        }
        //[HttpPost]
        //public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] OrderHeaderCreateDto orderHeaderlDto)
        //{
        //    try
        //    {
        //        OrderHeader order = new()
        //        {
        //            AppUserId = orderHeaderlDto.AppUserId,
        //            PickUpEmail = orderHeaderlDto.PickUpEmail,
        //            PickUpName = orderHeaderlDto.PickUpName,
        //            PickUpPhoneNumber = orderHeaderlDto.PickUpPhoneNumber,
        //            OrderTotal = orderHeaderlDto.OrderTotal,
        //            OrderDate = DateTime.Now,
        //            StripePaymentIntentId = orderHeaderlDto.StripePaymentIntentId,
        //            TotalItems = orderHeaderlDto.TotalItems,
        //            Status = String.IsNullOrEmpty(orderHeaderlDto.Status) ? SD.status_pending : orderHeaderlDto.Status,
        //        };
        //        if (ModelState.IsValid)
        //        {
        //            _db.OrderHeaders.Add(order);
        //            _db.SaveChanges();
        //            foreach (var orderDetailDto in orderHeaderlDto.OrderDetailDto)
        //            {
        //                OrderDetail orderDetail = new()
        //                {
        //                    OrderHeaderId = order.OrderHeaderId,
        //                    ItemName = orderDetailDto.ItemName,
        //                    MenuItemId = orderDetailDto.MenuItemId,
        //                    Price = orderDetailDto.Price,
        //                    Quantity = orderDetailDto.Quantity,
        //                };
        //                _db.OrderDetails.Add(orderDetail);
        //            }
        //            _db.SaveChanges();
        //            _apiResponse.Result = order;
        //            order.OrderDetail = null;
        //            _apiResponse.StatusCode = HttpStatusCode.Created;
        //            return Ok(_apiResponse);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _apiResponse.IsSuccess = false;
        //        _apiResponse.ErrorMessages = new List<string>() { e.ToString() };
        //    }
        //    return _apiResponse;
        //}
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] OrderHeaderCreateDto orderHeaderDTO)
        {
            try
            {
                OrderHeader order = new()
                {
                    AppUserId = orderHeaderDTO.AppUserId,
                    PickUpEmail = orderHeaderDTO.PickUpEmail,
                    PickUpName = orderHeaderDTO.PickUpName,
                    PickUpPhoneNumber = orderHeaderDTO.PickUpPhoneNumber,
                    OrderTotal = orderHeaderDTO.OrderTotal,
                    OrderDate = DateTime.Now,
                    StripePaymentIntentId = orderHeaderDTO.StripePaymentIntentId,
                    TotalItems = orderHeaderDTO.TotalItems,
                    Status = String.IsNullOrEmpty(orderHeaderDTO.Status) ? SD.status_pending : orderHeaderDTO.Status,
                };

                if (ModelState.IsValid)
                {
                    _db.OrderHeaders.Add(order);
                    _db.SaveChanges();
                    foreach (var orderDetailDTO in orderHeaderDTO.OrderDetailsDTO)
                    {
                        OrderDetail orderDetails = new()
                        {
                            OrderHeaderId = order.OrderHeaderId,
                            ItemName = orderDetailDTO.ItemName,
                            MenuItemId = orderDetailDTO.MenuItemId,
                            Price = orderDetailDTO.Price,
                            Quantity = orderDetailDTO.Quantity,
                        };
                        _db.OrderDetails.Add(orderDetails);
                    }
                    _db.SaveChanges();
                    _apiResponse.Result = order;
                    order.OrderDetail = null;
                    _apiResponse.StatusCode = HttpStatusCode.Created;
                    return Ok(_apiResponse);
                }
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _apiResponse;
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse>> Update(int id, [FromBody] OrderHeaderUpdateDto orderHeaderUpdateDto)
        {
            try
            {
                if (orderHeaderUpdateDto == null || id != orderHeaderUpdateDto.OrderHeaderId)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                OrderHeader orderExisting = _db.OrderHeaders.FirstOrDefault(x => x.OrderHeaderId == id);
                if (orderExisting == null)
                {
                    return BadRequest();
                }
                if (!string.IsNullOrEmpty(orderHeaderUpdateDto.PickUpName))
                {
                    orderExisting.PickUpName = orderHeaderUpdateDto.PickUpName;
                }
                if (!string.IsNullOrEmpty(orderHeaderUpdateDto.PickUpPhoneNumber))
                {
                    orderExisting.PickUpPhoneNumber = orderHeaderUpdateDto.PickUpPhoneNumber;
                }
                if (!string.IsNullOrEmpty(orderHeaderUpdateDto.PickUpEmail))
                {
                    orderExisting.PickUpEmail = orderHeaderUpdateDto.PickUpEmail;
                }
                if (!string.IsNullOrEmpty(orderHeaderUpdateDto.Status))
                {
                    orderExisting.Status = orderHeaderUpdateDto.Status;
                }
                if (!string.IsNullOrEmpty(orderHeaderUpdateDto.StripePaymentIntentId))
                {
                    orderExisting.StripePaymentIntentId = orderHeaderUpdateDto.StripePaymentIntentId;
                }
                _db.SaveChanges();
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }
            catch (Exception e)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string>() { e.ToString() };
            }
            return _apiResponse;
        }
    }
}
