using SpletnaTrgovinaDiploma.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SpletnaTrgovinaDiploma.Data.ViewModels;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public interface IOrdersService
    {
        IEnumerable<Order> GetOrdersByUserAsync(ClaimsPrincipal user);

        Task<Order> GetOrderByIdAndRoleAsync(int orderId, ClaimsPrincipal user);

        Task StoreOrderAsync(ShippingAndPaymentViewModel shippingAndPaymentViewModel, IEnumerable<ShoppingCartItem> items, ClaimsPrincipal user);

        Task UpdateOrderStatus(int orderId, OrderStatus oldStatus, OrderStatus newStatus, string comment, ClaimsPrincipal user);
    }
}
