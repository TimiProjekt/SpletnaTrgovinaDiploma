using System;
using SpletnaTrgovinaDiploma.Data.ViewModels;

namespace SpletnaTrgovinaDiploma.Helpers
{
    public static class ShippingOptionExtensions
    {
        public static string GetText(this ShippingOption shippingOption)
        {
            var text = shippingOption switch
            {
                ShippingOption.PriorityHomeShipping => "Priority Home Shipping",
                ShippingOption.StandardHomeShipping => "Standard Home Shipping",
                ShippingOption.Paketomat => "Paketomat",
                _ => throw new NotSupportedException()
            };

            return text;
        }
    }
}