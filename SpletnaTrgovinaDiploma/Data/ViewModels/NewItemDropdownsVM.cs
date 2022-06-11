using SpletnaTrgovinaDiploma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class NewItemDropdownsVM
    {
        public NewItemDropdownsVM()
        {
            Brands = new List<Brand>();
        }

        public List<Brand> Brands { get; set; }
    }
}
