using SpletnaTrgovinaDiploma.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpletnaTrgovinaDiploma.Data.ViewModels;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public interface IOrdersService
    {
        Task StoreOrderAsync(ShippingAndPaymentViewModel shippingAndPaymentViewModel, List<ShoppingCartItem> items, string userId);

        Task<List<Order>> GetOrdersByUserIdAndRoleAsync(string userId, string userRole);

        Order GetOrderByIdAndRole(int orderId, string userRole);
    }
}
