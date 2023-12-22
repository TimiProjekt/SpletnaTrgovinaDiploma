using System.ComponentModel.DataAnnotations;
using SpletnaTrgovinaDiploma.Data.Base;

namespace SpletnaTrgovinaDiploma.Models
{
    public class ItemDescription : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
