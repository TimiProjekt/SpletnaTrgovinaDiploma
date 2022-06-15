using SpletnaTrgovinaDiploma.Models;
using System.Collections.Generic;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class BrandDropdownViewModel
    {
        public List<Brand> Brands { get; set; }

        public BrandDropdownViewModel()
        {
            Brands = new List<Brand>();
        }
    }
}
