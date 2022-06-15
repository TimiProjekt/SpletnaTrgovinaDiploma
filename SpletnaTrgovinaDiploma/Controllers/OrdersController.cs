using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpletnaTrgovinaDiploma.Data.Cart;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IItemsService itemsService;
        private readonly ShoppingCart shoppingCart;
        private readonly IOrdersService ordersService;

        public OrdersController(IItemsService itemsService, ShoppingCart shoppingCart, IOrdersService ordersService)
        {
            this.itemsService = itemsService;
            this.shoppingCart = shoppingCart;
            this.ordersService = ordersService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var orders = await ordersService.GetOrdersByUserIdAndRoleAsync(userId, userRole);
            return View(orders);
        }

        public IActionResult ShoppingCart()
        {
            var items = shoppingCart.GetShoppingCartItems();
            shoppingCart.ShoppingCartItems = items;

            var response = new ShoppingCartViewModel()
            {
                ShoppingCart = shoppingCart,
                ShoppingCartTotal = shoppingCart.GetShoppingCartTotal()
            };

            return View(response);
        }

        public async Task<IActionResult> AddItemToShoppingCart(int id)
        {
            var item = await itemsService.GetItemByIdAsync(id);

            if (item != null)
                shoppingCart.AddItemToCart(item);

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            var item = await itemsService.GetItemByIdAsync(id);

            if (item != null)
                shoppingCart.RemoveItemFromCart(item);

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> CompleteOrder()
        {
            var items = shoppingCart.GetShoppingCartItems();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmailAddress = User.FindFirstValue(ClaimTypes.Email);

            await ordersService.StoreOrderAsync(items, userId, userEmailAddress);
            await shoppingCart.ClearShoppingCartAsync();

            return View("OrderCompleted");
        }
    }
}
