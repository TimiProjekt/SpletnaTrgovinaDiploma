using SpletnaTrgovinaDiploma.Data;
using SpletnaTrgovinaDiploma.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Models
{
    public class Item:IEntityBase
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public decimal Price { get; set; }
        public string ImageURL { get; set; }
        public ItemCategory ItemCategory { get; set; }

        //Relationships
        public List<Brand_Item> Brands_Items { get; set; }
    }
}
