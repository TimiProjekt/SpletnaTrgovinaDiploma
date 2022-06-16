using System.ComponentModel.DataAnnotations;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class SettingsViewModel
    {
        public string UserName { get; set; }

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
        public int? CountryId { get; set; }
    }
}
