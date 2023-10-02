using SpletnaTrgovinaDiploma.Models;
using System.ComponentModel.DataAnnotations;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class UnregisteredViewModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        public string PersonName { get; set; }

        [Display(Name = "Surname")]
        [Required(ErrorMessage = "Surname is required")]
        public string PersonSurname { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required")]
        public string EmailAddress { get; set; }

        [Display(Name = "Telephone number")]
        [Required(ErrorMessage = "Telephone number is required")]
        public string TelephoneNumber { get; set; }

        [Display(Name = "Street name")]
        [Required(ErrorMessage = "Street name is required")]
        public string StreetName { get; set; }

        [Display(Name = "House number")]
        [Required(ErrorMessage = "House number is required")]
        public string HouseNumber { get; set; }

        [Display(Name = "Zip code")]
        [Required(ErrorMessage = "Zip code is required")]
        public string ZipCode { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Country is required")]
        public int? CountryId { get; set; }
        public Country Country { get; set; }

        public bool HasAddress => !string.IsNullOrEmpty(StreetName) && !string.IsNullOrEmpty(HouseNumber) &&
                                  !string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(ZipCode) &&
                                  CountryId.HasValue;

        public string GetFullAddress =>
            HasAddress ? $"{StreetName} {HouseNumber}, {ZipCode} {City}, {Country?.Name}" : "No address";
    }
}
