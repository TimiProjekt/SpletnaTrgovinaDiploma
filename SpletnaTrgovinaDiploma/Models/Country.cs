using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SpletnaTrgovinaDiploma.Data.Base;

namespace SpletnaTrgovinaDiploma.Models
{
    [Table("Country")]
    public class Country : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
