
using SpletnaTrgovinaDiploma.Data.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpletnaTrgovinaDiploma.Models
{
    [Table("DeliveryInfo")]
    public class DeliveryInfo : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        public string PersonName { get; set; }

        public string PersonSurname { get; set; }

        public string EmailAddress { get; set; }

        public string TelephoneNumber { get; set; }

        public string StreetName { get; set; }

        public string HouseNumber { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public int? CountryId { get; set; }
        public Country Country { get; set; }

        public bool HasAddress => !string.IsNullOrEmpty(StreetName) && !string.IsNullOrEmpty(HouseNumber) &&
                                  !string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(ZipCode) &&
                                  CountryId.HasValue;

        public string GetFullAddress =>
            HasAddress ? $"{StreetName} {HouseNumber}, {ZipCode} {City}, {Country?.Name}" : "No address";

        public string FullName => PersonName + " " + PersonSurname;
    }
}
