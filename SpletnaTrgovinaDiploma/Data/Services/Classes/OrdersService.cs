using Microsoft.EntityFrameworkCore;
using SpletnaTrgovinaDiploma.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Helpers;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext context;

        public OrdersService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Order>> GetOrdersByUserAsync(ClaimsPrincipal user)
        {
            var orders = await context.Orders
                .Include(n => n.OrderItems)
                .ThenInclude(n => n.Item)
                .Include(n => n.User)
                .Include(n => n.StatusChangedLog)
                .ToListAsync();

            if (!user.IsUserAdmin())
                orders = orders.Where(n => n.UserId == user.GetUserId()).ToList();

            return orders;
        }

        public Order GetOrderByIdAndRole(int orderId, ClaimsPrincipal user)
        {
            if (!user.IsUserAdmin())
                return null;

            var order = context.Orders
                .SingleOrDefault(o => o.Id == orderId);

            return order;
        }

        public async Task UpdateOrderStatus(int orderId, OrderStatus oldStatus, OrderStatus newStatus, string comment, ClaimsPrincipal user)
        {
            var order = context.Orders
                .SingleOrDefault(o => o.Id == orderId);

            if (order == null)
                return;

            order.Status = newStatus;

            InsertOrderStatusChangedLog(orderId, oldStatus, newStatus, comment, user);

            await context.SaveChangesAsync();
        }

        void InsertOrderStatusChangedLog(int orderId, OrderStatus oldStatus, OrderStatus newStatus, string comment, ClaimsPrincipal user)
        {
            var newLog = new OrderStatusChangedLog(orderId, oldStatus, newStatus, comment, user.GetUserId());

            context.OrderStatusChangedLog.Add(newLog);
        }

        public async Task StoreOrderAsync(ShippingAndPaymentViewModel shippingAndPaymentViewModel, List<ShoppingCartItem> items, ClaimsPrincipal user)
        {
            var order = new Order()
            {
                UserId = user.GetUserId(),
                DeliveryEmailAddress = shippingAndPaymentViewModel.EmailAddress,
                FullName = shippingAndPaymentViewModel.FullName,
                DeliveryPhoneNumber = shippingAndPaymentViewModel.PhoneNumber,
                StreetName = shippingAndPaymentViewModel.StreetName,
                HouseNumber = shippingAndPaymentViewModel.HouseNumber,
                City = shippingAndPaymentViewModel.City,
                ZipCode = shippingAndPaymentViewModel.ZipCode,
                ShippingOption = shippingAndPaymentViewModel.ShippingOption,
                PaymentOption = shippingAndPaymentViewModel.PaymentOption,
                Country = context.Countries.Single(c => c.Id == shippingAndPaymentViewModel.CountryId),
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

            InsertOrderStatusChangedLog(order.Id, OrderStatus.Processing, OrderStatus.Processing, "Initial order placed", user);
            await context.SaveChangesAsync();
        }
    }
}
