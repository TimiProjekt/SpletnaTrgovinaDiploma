using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpletnaTrgovinaDiploma.Models
{
    public class Order
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }

        public string Email { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}
