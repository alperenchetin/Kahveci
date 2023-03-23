using Kahveci_API.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kahveci_API.Dto
{
    public class OrderHeaderCreateDto
    {

        public string PickUpName { get; set; }

        public string PickUpPhoneNumber { get; set; }

        public string PickUpEmail { get; set; }


        public string AppUserId { get; set; }
        public double OrderTotal { get; set; }

        public string StripePaymentIntentId { get; set; }
        public string Status { get; set; }
        public int TotalItems { get; set; }

        public IEnumerable<OrderDetailCreateDto> OrderDetailDto { get; set; }
        public IEnumerable<OrderDetailCreateDto> OrderDetailsDTO { get; set; }
    }
}
