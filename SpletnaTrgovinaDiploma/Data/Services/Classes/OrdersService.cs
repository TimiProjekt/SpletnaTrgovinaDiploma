using Microsoft.EntityFrameworkCore;
using SpletnaTrgovinaDiploma.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpletnaTrgovinaDiploma.Data.ViewModels;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext context;

        public OrdersService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Order>> GetOrdersByUserIdAndRoleAsync(string userId, string userRole)
        {
            var orders = await context.Orders
                .Include(n => n.OrderItems)
                .ThenInclude(n => n.Item)
                .Include(n => n.User)
                .ToListAsync();

            if (userRole != "Admin")
                orders = orders.Where(n => n.UserId == userId).ToList();

            return orders;
        }

        public async Task StoreOrderAsync(UserInfoViewModel userInfoViewModel, List<ShoppingCartItem> items, string userId)
        {
            var order = new Order()
            {
                UserId = userId,
                DeliveryEmailAddress = userInfoViewModel.EmailAddress,
                FullName = userInfoViewModel.FullName,
                DeliveryPhoneNumber = userInfoViewModel.PhoneNumber,
                StreetName = userInfoViewModel.StreetName,
                HouseNumber = userInfoViewModel.HouseNumber,
                City = userInfoViewModel.City,
                ZipCode = userInfoViewModel.ZipCode,
                Country = context.Countries.Single(c => c.Id == userInfoViewModel.CountryId),
            };

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            foreach (var item in items)
            {
                var orderItem = new OrderItem()
                {
                    Amount = item.Amount,
                    ItemId = item.Item.Id,
                    OrderId = order.Id,
                    Price = item.Item.Price
                };
                await context.OrderItems.AddAsync(orderItem);
            }
            await context.SaveChangesAsync();
        }
    }
}
