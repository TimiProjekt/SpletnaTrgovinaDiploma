using System.ComponentModel.DataAnnotations.Schema;

namespace SpletnaTrgovinaDiploma.Models
{
    public class OrderItem
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }

        public int Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public Item Item { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
