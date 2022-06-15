using SpletnaTrgovinaDiploma.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public interface IOrdersService
    {
        Task StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress);
        Task<List<Order>> GetOrdersByUserIdAndRoleAsync(string userId, string userRole);
    }
}
