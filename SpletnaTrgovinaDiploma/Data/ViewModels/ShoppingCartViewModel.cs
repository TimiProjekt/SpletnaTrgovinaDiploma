using System.Collections.Generic;
using System.Linq;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<ShoppingCartItem> Items { get; set; }

        public decimal Total
            => Items
                .Select(n => n.Item.Price * n.Amount)
                .Sum();

        public decimal TotalWithoutVat
            => Total * 100 / 122;

        public int TotalAmountOfItems
            => Items
                .Select(i => i.Amount)
                .Sum();
    }
}
