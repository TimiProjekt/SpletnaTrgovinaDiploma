using System.Collections.Generic;
using System.Threading.Tasks;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public interface IShoppingCartService
    {
        public Task<List<ShoppingCartItem>> GetShoppingCartItems();

        public Task IncreaseItemInCartAsync(Item item, int byAmount);

        public Task DecreaseItemInCartAsync(Item item);

        public Task RemoveItemFromCartAsync(Item item);

        public Task SetItemAmountInCartAsync(Item item, int amount);

        public Task ClearShoppingCartAsync();
    }
}
