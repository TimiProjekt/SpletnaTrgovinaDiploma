using System.ComponentModel.DataAnnotations;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class UserInfoViewModel
    {
        public string EmailAddress { get; set; }

        [Display(Name = "Full name")]
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Display(Name = "Telephone number")]
        [Required(ErrorMessage = "Telephone number is required")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Street name")]
        [Required(ErrorMessage = "Street name is required")]
        public string StreetName { get; set; }

        [Display(Name = "House number")]
        [Required(ErrorMessage = "House number is required")]
        public string HouseNumber { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Display(Name = "Zip code")]
        [Required(ErrorMessage = "Zip code is required")]
        public string ZipCode { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Country is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a country")]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        public bool HasAddress => !string.IsNullOrEmpty(StreetName) && !string.IsNullOrEmpty(HouseNumber) &&
                                  !string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(ZipCode);

        public string GetFullAddress =>
            HasAddress ? $"{StreetName} {HouseNumber}, {ZipCode} {City}, {Country?.Name}" : "No address";
    }
}
