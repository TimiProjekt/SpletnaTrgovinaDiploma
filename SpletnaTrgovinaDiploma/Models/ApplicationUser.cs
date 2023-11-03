using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SpletnaTrgovinaDiploma.Models
{
    public class ApplicationUser : IdentityUser, IDeliveryInfo
    {
        [Display(Name = "Email address")]
        public string EmailAddress => UserName;

        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Display(Name = "Email address")]
        public string DeliveryEmailAddress { get; set; }

        public string DeliveryPhoneNumber { get; set; }

        public string StreetName { get; set; }

        public string HouseNumber { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }
        [IgnoreDataMember]
        public Country Country { get; set; }

        public bool HasAddress => !string.IsNullOrEmpty(StreetName) && !string.IsNullOrEmpty(HouseNumber) &&
                                  !string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(ZipCode);

        public string GetFullAddress =>
            HasAddress ? $"{StreetName} {HouseNumber}, {ZipCode} {City}, {Country?.Name}" : "No address";
    }
}
