using System.ComponentModel.DataAnnotations;
using SpletnaTrgovinaDiploma.Data.Cart;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class ShoppingCartViewModel
    {
        public ShoppingCart ShoppingCart { get; set; }

        public decimal ShoppingCartTotal { get; set; }
    }
}
