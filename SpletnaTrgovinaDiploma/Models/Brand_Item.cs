using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Models
{
    public class Brand_Item
    {

        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }
    }
}
