using SpletnaTrgovinaDiploma.Data.Services.Classes;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class ShippingAndPaymentViewModel : UserInfoViewModel
    {
        public ShoppingCartViewModel ShoppingCart { get; set; }

        public ShippingOption ShippingOption { get; set; }

        public PaymentOption PaymentOption { get; set; }
    }

    public enum ShippingOption
    {
        PriorityHomeShipping,
        StandardHomeShipping,
        Paketomat
    }

    public enum PaymentOption
    {
        CreditCard,
        BankTransfer,
        Cash
    }
}
