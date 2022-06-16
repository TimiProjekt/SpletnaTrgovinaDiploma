using SpletnaTrgovinaDiploma.Data.Cart;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class ShoppingCartViewModel
    {
        public ShoppingCart ShoppingCart { get; set; }

        public decimal ShoppingCartTotal { get; set; }

        public bool HasAddress { get; set; }
    }
}
