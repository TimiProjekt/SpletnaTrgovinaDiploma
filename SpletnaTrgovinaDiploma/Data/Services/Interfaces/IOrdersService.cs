using SpletnaTrgovinaDiploma.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SpletnaTrgovinaDiploma.Data.ViewModels;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public interface IOrdersService
    {
        Task StoreOrderAsync(ShippingAndPaymentViewModel shippingAndPaymentViewModel, List<ShoppingCartItem> items, ClaimsPrincipal user);

        Task<List<Order>> GetOrdersByUserAsync(ClaimsPrincipal user);

        Order GetOrderByIdAndRole(int orderId, ClaimsPrincipal user);

        Task UpdateOrderStatus(int orderId, OrderStatus oldStatus, OrderStatus newStatus, string comment, ClaimsPrincipal user);
    }
}
