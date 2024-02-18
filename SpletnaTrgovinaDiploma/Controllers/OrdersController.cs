using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SpletnaTrgovinaDiploma.Data.Cart;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SpletnaTrgovinaDiploma.Models;
using SpletnaTrgovinaDiploma.Helpers;

namespace SpletnaTrgovinaDiploma.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ICountryService countryService;
        private readonly IItemsService itemsService;
        private readonly IOrdersService ordersService;
        private readonly ShoppingCart shoppingCart;
        private readonly SignInHelper signInHelper;

        public OrdersController(
            ICountryService countryService,
            IItemsService itemsService,
            IOrdersService ordersService,
            ShoppingCart shoppingCart,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.countryService = countryService;
            this.itemsService = itemsService;
            this.ordersService = ordersService;
            this.shoppingCart = shoppingCart;

            signInHelper = new SignInHelper(userManager, signInManager);
        }

        public async Task<IActionResult> Index()
            => await Filter(null);

        public async Task<IActionResult> Filter(string searchString)
        {
            var allOrders = await ordersService.GetOrdersByUserAsync(User);

            if (!string.IsNullOrEmpty(searchString))
            {
                static bool ContainsCaseInsensitiveString(string source, string contains)
                    => source?.ToUpper().Contains(contains.ToUpper()) ?? false;

                var filteredOrders = allOrders.Where(n => ContainsCaseInsensitiveString(n.Id.ToString(), searchString) || ContainsCaseInsensitiveString(n.DeliveryEmailAddress, searchString));

                ViewData.SetPageDetails("Search result", $"Search result for \"{searchString}\"");
                return View("Index", filteredOrders);
            }

            ViewData.SetPageDetails("Orders", "Orders");
            return View("Index", allOrders);
        }

        public IActionResult GetById(int id)
        {
            var order = ordersService.GetOrderByIdAndRole(id, User);
            if (order == null)
                return RedirectToAction("Index", "Items");

            DropdownUtil.LoadCountriesDropdownData(countryService, ViewBag);
            return View(order);
        }

        public IActionResult ShoppingCart()
        {
            var items = shoppingCart.GetShoppingCartItems();
            shoppingCart.ShoppingCartItems = items;

            var response = new ShoppingCartViewModel()
            {
                ShoppingCart = shoppingCart,
                ShoppingCartTotal = shoppingCart.GetShoppingCartTotal(),
            };

            return View(response);
        }

        public async Task<IActionResult> IncreaseItemInShoppingCart(int id, int byAmount = 1)
        {
            var item = await itemsService.GetItemByIdAsync(id);
            if (item != null)
                shoppingCart.IncreaseItemInCart(item, byAmount);

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> DecreaseItemInShoppingCart(int id)
        {
            var item = await itemsService.GetItemByIdAsync(id);

            if (item != null)
                shoppingCart.DecreaseItemInCart(item);

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            var item = await itemsService.GetItemByIdAsync(id);

            if (item != null)
                shoppingCart.RemoveItemFromCart(item);

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task SetItemAmountInShoppingCart(int id, int amount)
        {
            var item = await itemsService.GetItemByIdAsync(id);

            if (item != null)
                shoppingCart.SetItemAmountInCart(item, amount);
        }

        public IActionResult DeliveryInfo()
        {
            if (User.IsLoggedIn())
                return RedirectToAction(nameof(ShippingAndPayment));

            var loginViewModel = new LoginViewModel();
            DropdownUtil.LoadCountriesDropdownData(countryService, ViewBag);
            return View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeliveryInfo(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                DropdownUtil.LoadCountriesDropdownData(countryService, ViewBag);
                return View(loginViewModel);
            }

            var loginResult = await signInHelper.Login(loginViewModel);
            if (loginResult.Succeeded)
                return RedirectToAction(nameof(ShippingAndPayment));

            TempData.SetError("Wrong credentials. Please, try again!");
            return View(loginViewModel);
        }

        public IActionResult ShippingAndPayment()
        {
            var appUser = signInHelper.GetApplicationUser(User);
            var myShoppingCart = shoppingCart.GetShoppingCartWithItems();
            var shoppingCartTotal = myShoppingCart.GetShoppingCartTotal();

            var response = new ShippingAndPaymentViewModel()
            {
                ShoppingCart = myShoppingCart,
                ShoppingCartTotal = shoppingCartTotal,
                ShoppingCartTotalWithoutVat = shoppingCartTotal * 100 / 122,
                EmailAddress = !string.IsNullOrEmpty(appUser?.DeliveryEmailAddress)
                    ? appUser.DeliveryEmailAddress
                    : appUser?.Email ?? "",
                FullName = appUser?.FullName ?? "",
                PhoneNumber = appUser?.PhoneNumber ?? "",
                StreetName = appUser?.StreetName ?? "",
                HouseNumber = appUser?.HouseNumber ?? "",
                City = appUser?.City ?? "",
                ZipCode = appUser?.ZipCode ?? "",
                CountryId = appUser?.CountryId ?? 1
            };

            DropdownUtil.LoadCountriesDropdownData(countryService, ViewBag);
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> ShippingAndPayment(ShippingAndPaymentViewModel shippingAndPaymentViewModel)
        {
            if (!ModelState.IsValid)
            {
                DropdownUtil.LoadCountriesDropdownData(countryService, ViewBag);
                return View(shippingAndPaymentViewModel);
            }

            var myShoppingCart = shoppingCart.GetShoppingCartWithItems();
            shippingAndPaymentViewModel.ShoppingCart = myShoppingCart;
            shippingAndPaymentViewModel.ShoppingCartTotal = myShoppingCart.GetShoppingCartTotal();

            await ordersService.StoreOrderAsync(shippingAndPaymentViewModel, myShoppingCart.ShoppingCartItems, User);

            shippingAndPaymentViewModel.SendOrderConfirmationEmail(myShoppingCart);

            await myShoppingCart.ClearShoppingCartAsync();

            return View(
                "Success",
                new SuccessViewModel(
                    "Order completed successfully.",
                    "You can check all your orders in the Orders section of your profile.",
                    "Thank you!"));
        }

        public IActionResult EditStatus(int id)
        {
            if (!User.IsUserAdmin())
                return View("NotFound");

            var order = ordersService.GetOrderByIdAndRole(id, User);
            if (order == null)
                return RedirectToAction("Index", "Orders");

            var orderStatus = new OrderStatusViewModel()
            {
                OrderId = id,
                CurrentStatus = order.Status
            };

            DropdownUtil.LoadStatusDropdownData(order, ViewBag);
            return View(orderStatus);
        }

        [HttpPost]
        public async Task<IActionResult> EditStatus(OrderStatusViewModel orderStatus)
        {
            if (!User.IsUserAdmin())
                return View("NotFound");

            var order = ordersService.GetOrderByIdAndRole(orderStatus.OrderId, User);
            if (order == null)
                return RedirectToAction("Index", "Orders");

            if (!ModelState.IsValid)
            {
                DropdownUtil.LoadStatusDropdownData(order, ViewBag);
                return View(orderStatus);
            }

            if (orderStatus.CurrentStatus.HasValue && orderStatus.NewStatus.HasValue)
            {
                await ordersService.UpdateOrderStatus(
                    orderStatus.OrderId,
                    orderStatus.CurrentStatus.Value,
                    orderStatus.NewStatus.Value,
                    orderStatus.Comment,
                    User);
            }

            return RedirectToAction(nameof(GetById), new { id = orderStatus.OrderId });
        }
    }
}
