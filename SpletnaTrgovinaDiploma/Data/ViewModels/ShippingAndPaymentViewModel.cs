using SpletnaTrgovinaDiploma.Data.Cart;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class ShippingAndPaymentViewModel
    {
        public ShoppingCart ShoppingCart { get; set; }

        public decimal ShoppingCartTotal { get; set; }

        public UserInfoViewModel UserInfoViewModel { get; set; }
    }
}
