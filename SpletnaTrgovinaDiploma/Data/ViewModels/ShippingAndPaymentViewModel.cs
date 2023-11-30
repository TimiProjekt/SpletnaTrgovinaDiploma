using SpletnaTrgovinaDiploma.Data.Cart;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class ShippingAndPaymentViewModel : UserInfoViewModel
    {
        public ShoppingCart ShoppingCart { get; set; }

        public decimal ShoppingCartTotal { get; set; }

        public decimal ShoppingCartTotalWithoutVat { get; set; }

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
        Paypal,
        Cash
    }
}
