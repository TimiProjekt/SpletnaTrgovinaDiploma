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
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }

        public ShoppingCart(AppDbContext context)
        {
            this.context = context;
        }

        public static ShoppingCart GetShoppingCart(IServiceProvider services)
        {
            var session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
            var context = services.GetService<AppDbContext>();

            var cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);

            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        public void AddItemToCart(Item item)
        {
            var shoppingCartItem = context.ShoppingCartItems.FirstOrDefault(n => n.Item.Id == item.Id && n.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem()
                {
                    ShoppingCartId = ShoppingCartId,
                    Item = item,
                    Amount = 1
                };

                context.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Amount++;
            }
            context.SaveChanges();
        }

        public void RemoveItemFromCart(Item item)
        {
            var shoppingCartItem = context.ShoppingCartItems.FirstOrDefault(n => n.Item.Id == item.Id && n.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Amount > 1)
                {
                    shoppingCartItem.Amount--;
                }
                else
                {
                    context.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }
            context.SaveChanges();
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ?? (ShoppingCartItems = context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).Include(n => n.Item).ToList());
        }

        public decimal GetShoppingCartTotal() => context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).Select(n => n.Item.Price * n.Amount).Sum();

        public async Task ClearShoppingCartAsync()
        {
            var items = await context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).ToListAsync();
            context.ShoppingCartItems.RemoveRange(items);
            await context.SaveChangesAsync();
        }

        public bool IsItemInCart(Item item)
        {
            var shoppingCartItem = context.ShoppingCartItems.FirstOrDefault(n => n.Item.Id == item.Id && n.ShoppingCartId == ShoppingCartId);
            return shoppingCartItem != null;
        }
    }
}
