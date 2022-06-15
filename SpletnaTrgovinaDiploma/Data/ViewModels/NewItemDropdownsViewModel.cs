using SpletnaTrgovinaDiploma.Models;
using System.Collections.Generic;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class NewItemDropdownsViewModel
    {
        public NewItemDropdownsViewModel()
        {
            Brands = new List<Brand>();
        }

        public List<Brand> Brands { get; set; }
    }
}
