using System;
using SpletnaTrgovinaDiploma.Data.Base;
using System.ComponentModel.DataAnnotations;

namespace SpletnaTrgovinaDiploma.Models
{
    public class OrderStatusChangedLog : IEntityBase
    {
        public OrderStatusChangedLog(int orderId, OrderStatus oldStatus, OrderStatus newStatus, string comment, string userId)
        {
            OrderId = orderId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            Comment = comment;
            UserId = userId;

            Timestamp = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [Display(Name = "Order")]
        public int OrderId { get; set; }

        [Display(Name = "From status")]
        public OrderStatus OldStatus { get; set; }

        [Display(Name = "To status")]
        public OrderStatus NewStatus { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "User")]
        public string UserId { get; set; }

        [Display(Name = "Timestamp")]
        public DateTime Timestamp { get; set; }
    }
}