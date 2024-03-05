using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Data.Services.Classes
{
    public class ShoppingCartService
    {
        private readonly AppDbContext context;

        public string ShoppingCartId { get; set; }

        public static ShoppingCartService GetShoppingCart(IServiceProvider services)
        {
            var session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
            var context = services.GetService<AppDbContext>();

            var cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();
            session?.SetString("CartId", cartId);

            return new ShoppingCartService(context) { ShoppingCartId = cartId };
        }

        ShoppingCartService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ShoppingCartViewModel> GetShoppingCartViewModel()
            => new()
            {
                Items = await GetShoppingCartItems()
            };

        async Task<List<ShoppingCartItem>> GetShoppingCartItems()
            => await context.ShoppingCartItems
                .Where(n => n.ShoppingCartId == ShoppingCartId)
                .Include(n => n.Item)
                .ToListAsync();

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
