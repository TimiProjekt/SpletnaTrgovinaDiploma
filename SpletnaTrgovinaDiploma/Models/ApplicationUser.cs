using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SpletnaTrgovinaDiploma.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        public string StreetName { get; set; }

        public string HouseNumber { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public int? CountryId { get; set; }
        public Country Country { get; set; }

        public bool HasAddress => !string.IsNullOrEmpty(StreetName) && !string.IsNullOrEmpty(HouseNumber) &&
                                  !string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(ZipCode) &&
                                  CountryId.HasValue;
    }
}
