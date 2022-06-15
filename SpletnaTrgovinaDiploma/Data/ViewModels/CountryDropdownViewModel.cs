using SpletnaTrgovinaDiploma.Models;
using System.Collections.Generic;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class CountryDropdownViewModel
    {
        public List<Country> Countries { get; set; }

        public CountryDropdownViewModel()
        {
            Countries = new List<Country>();
        }
    }
}
