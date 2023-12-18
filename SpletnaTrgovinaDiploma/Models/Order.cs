using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SpletnaTrgovinaDiploma.Models
{
    public class Order : IDeliveryInfo
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }

        public string FullName { get; set; }

        public string DeliveryEmailAddress { get; set; }

        public string DeliveryPhoneNumber { get; set; }

        public string StreetName { get; set; }

        public string HouseNumber { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public int ShippingOption { get; set; }

        public int PaymentOption { get; set; }

        [ForeignKey(nameof(Country))]
        public int? CountryId { get; set; }
        [IgnoreDataMember]
        public Country Country { get; set; }

        public bool HasAddress => !string.IsNullOrEmpty(StreetName) && !string.IsNullOrEmpty(HouseNumber) &&
                                  !string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(ZipCode);

        public string GetFullAddress =>
            HasAddress ? $"{StreetName} {HouseNumber}, {ZipCode} {City}, {Country?.Name}" : "No address";

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        public List<OrderItem> OrderItems { get; set; }


    }
}
