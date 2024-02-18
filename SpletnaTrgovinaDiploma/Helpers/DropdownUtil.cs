using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Helpers
{
    public static class DropdownUtil
    {
        public static void LoadStatusDropdownData(Order order, dynamic viewBag)
            => viewBag.Statuses = order.Status.GetDropdownValues(order.PaymentOption);

        public static async Task LoadBrandsDropdownData(IBrandsService brandService, dynamic viewBag)
        {
            var itemDropdownsData = await brandService.GetDropdownValuesAsync();

            viewBag.Brands = new SelectList(itemDropdownsData.Brands, "Id", "Name");
        }

        public static void LoadCountriesDropdownData(ICountryService countryService, dynamic viewBag)
        {
            var defaultEmptyValue = new Country { Id = 0, Name = "-- Select a country --" };
            var itemDropdownsData = countryService.GetDropdownValuesAsync().Result;
            itemDropdownsData.Countries.Insert(0, defaultEmptyValue);

            viewBag.Countries = new SelectList(itemDropdownsData.Countries, "Id", "Name");
        }
    }
}
