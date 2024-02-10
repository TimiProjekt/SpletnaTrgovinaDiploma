using System.ComponentModel.DataAnnotations;

namespace SpletnaTrgovinaDiploma.Models
{
    public class OrderStatusViewModel
    {
        [Display(Name = "Order id")]
        public int OrderId { get; set; }

        [Display(Name = "Current order status")]
        public OrderStatus CurrentStatus { get; set; }

        [Display(Name = "New order status")]
        [Required(ErrorMessage = "New order status is required")]
        public OrderStatus NewStatus { get; set; }
    }
}
