using System;
using SpletnaTrgovinaDiploma.Data.ViewModels;

namespace SpletnaTrgovinaDiploma.Helpers
{
    public static class PaymentOptionExtensionMethod
    {
        public static string GetText(this PaymentOption paymentOption)
        {
            var text = paymentOption switch
            {
                PaymentOption.CreditCard => "Visa/Mastercard online payment",
                PaymentOption.BankTransfer => "Funds transfer",
                PaymentOption.Cash => "Payment after arrival",
                _ => throw new NotSupportedException()
            };

            return text;
        }
    }
}