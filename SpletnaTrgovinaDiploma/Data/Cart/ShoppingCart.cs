using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpletnaTrgovinaDiploma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Data.Cart
{
    public class ShoppingCart
    {
        private readonly AppDbContext context;

        public string ShoppingCartId { get; set; }

        IQueryable<ShoppingCartItem> CurrentShoppingCartItems
            => context.ShoppingCartItems
                .Where(n => n.ShoppingCartId == ShoppingCartId);

        public IEnumerable<ShoppingCartItem> ShoppingCartItems
            => CurrentShoppingCartItems
                .Include(n => n.Item);

        public async Task<decimal> GetShoppingCartTotalAsync()
            => await CurrentShoppingCartItems
                .Select(n => n.Item.Price * n.Amount)
                .SumAsync();

        public async Task<int> GetTotalAmountOfItemsAsync()
            => await CurrentShoppingCartItems
                .Select(i => i.Amount)
                .SumAsync();

        public ShoppingCart(AppDbContext context)
        {
            this.context = context;
        }

        public static ShoppingCart GetShoppingCart(IServiceProvider services)
        {
            var session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
            var context = services.GetService<AppDbContext>();

            var cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();
            session?.SetString("CartId", cartId);

            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        public async Task IncreaseItemInCartAsync(Item item, int byAmount)
        {
            var shoppingCartItem = await context.ShoppingCartItems
                .FirstOrDefaultAsync(n => n.Item.Id == item.Id && n.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem()
                {
                    ShoppingCartId = ShoppingCartId,
                    Item = item,
                    Amount = byAmount
                };

                await context.ShoppingCartItems.AddAsync(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Amount += byAmount;
            }

            await context.SaveChangesAsync();
        }

        public async Task DecreaseItemInCartAsync(Item item)
        {
            var shoppingCartItem = await context.ShoppingCartItems
                .FirstOrDefaultAsync(n => n.Item.Id == item.Id && n.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem is { Amount: > 1 })
                shoppingCartItem.Amount--;

            await context.SaveChangesAsync();
        }

        public async Task RemoveItemFromCartAsync(Item item)
        {
            var shoppingCartItem = await context.ShoppingCartItems
                .FirstOrDefaultAsync(n => n.Item.Id == item.Id && n.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem != null)
                context.ShoppingCartItems.Remove(shoppingCartItem);

            await context.SaveChangesAsync();
        }

        public async Task SetItemAmountInCartAsync(Item item, int amount)
        {
            var shoppingCartItem = await context.ShoppingCartItems
                .FirstOrDefaultAsync(n => n.Item.Id == item.Id && n.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem != null)
                shoppingCartItem.Amount = amount;

            await context.SaveChangesAsync();
        }

        public async Task ClearShoppingCartAsync()
        {
            var items = await context.ShoppingCartItems
                .Where(n => n.ShoppingCartId == ShoppingCartId)
                .ToListAsync();

            context.ShoppingCartItems.RemoveRange(items);

            await context.SaveChangesAsync();
        }
    }
}
