using System.ComponentModel.DataAnnotations;
using SpletnaTrgovinaDiploma.Data.Cart;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class ShippingAndPaymentViewModel
    {
        public ShoppingCart ShoppingCart { get; set; }

        public decimal ShoppingCartTotal { get; set; }

        [Display(Name = "Street name")]
        public string StreetName { get; set; }

        [Display(Name = "House number")]
        public string HouseNumber { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Zip code")]
        public string ZipCode { get; set; }

        [Display(Name = "Country")]
        public string CountryName { get; set; }

        public bool HasAddress { get; set; }
    }
}
