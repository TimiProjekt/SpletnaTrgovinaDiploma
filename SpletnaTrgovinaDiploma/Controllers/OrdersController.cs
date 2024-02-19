using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SpletnaTrgovinaDiploma.Data.Cart;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SpletnaTrgovinaDiploma.Models;
using SpletnaTrgovinaDiploma.Helpers;
using X.PagedList;

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

        public IActionResult Index(string currentFilter, string searchString, int page = 1)
        {
            var filteredOrders = GetFilteredOrders(currentFilter, searchString, page);
            return View(filteredOrders);
        }

        IPagedList<Order> GetFilteredOrders(string currentFilter, string searchString, int page)
        {
            const int itemsPerPage = 12;

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;
            var allOrders = ordersService
                .GetOrdersByUser(User);

            if (!string.IsNullOrEmpty(searchString))
            {
                static bool ContainsCaseInsensitiveString(string source, string contains)
                    => source?.ToUpper().Contains(contains.ToUpper()) ?? false;

                var filteredResult = allOrders.Where(n => ContainsCaseInsensitiveString(n.Id.ToString(), searchString) || ContainsCaseInsensitiveString(n.DeliveryEmailAddress, searchString));

                ViewData.SetPageDetails("Search result", $"Search result for \"{searchString}\"");
                return filteredResult.ToPagedList(page, itemsPerPage);
            }

            ViewData.SetPageDetails("Home page", "Home page of Gaming svet");
            return allOrders.ToPagedList(page, itemsPerPage);
        }

        public async Task<IActionResult> GetById(int id)
        {
            var order = await ordersService.GetOrderByIdAndRoleAsync(id, User);
            if (order == null)
                return RedirectToAction("Index", "Items");

            DropdownUtil.LoadCountriesDropdownData(countryService, ViewBag);
            return View(order);
        }

        public async Task<IActionResult> ShoppingCart()
        {
            var response = new ShoppingCartViewModel()
            {
                ShoppingCart = shoppingCart,
                ShoppingCartTotal = await shoppingCart.GetShoppingCartTotalAsync(),
            };

            return View(response);
        }

        public async Task<IActionResult> IncreaseItemInShoppingCart(int id, int byAmount = 1)
        {
            var item = await itemsService.GetItemByIdAsync(id);
            if (item != null)
                await shoppingCart.IncreaseItemInCartAsync(item, byAmount);

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> DecreaseItemInShoppingCart(int id)
        {
            var item = await itemsService.GetItemByIdAsync(id);

            if (item != null)
                await shoppingCart.DecreaseItemInCartAsync(item);

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            var item = await itemsService.GetItemByIdAsync(id);

            if (item != null)
                await shoppingCart.RemoveItemFromCartAsync(item);

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task SetItemAmountInShoppingCart(int id, int amount)
        {
            var item = await itemsService.GetItemByIdAsync(id);

            if (item != null)
                await shoppingCart.SetItemAmountInCartAsync(item, amount);
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

        public async Task<IActionResult> ShippingAndPayment()
        {
            var appUser = signInHelper.GetApplicationUser(User);
            var shoppingCartTotal = await shoppingCart.GetShoppingCartTotalAsync();

            var response = new ShippingAndPaymentViewModel()
            {
                ShoppingCart = shoppingCart,
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

            shippingAndPaymentViewModel.ShoppingCart = shoppingCart;
            shippingAndPaymentViewModel.ShoppingCartTotal = await shoppingCart.GetShoppingCartTotalAsync();

            await ordersService.StoreOrderAsync(shippingAndPaymentViewModel, shoppingCart.ShoppingCartItems, User);

            shippingAndPaymentViewModel.SendOrderConfirmationEmail(shoppingCart);

            await shoppingCart.ClearShoppingCartAsync();

            return View(
                "Success",
                new SuccessViewModel(
                    "Order completed successfully.",
                    "You can check all your orders in the Orders section of your profile.",
                    "Thank you!"));
        }

        public async Task<IActionResult> EditStatus(int id)
        {
            if (!User.IsUserAdmin())
                return View("NotFound");

            var order = await ordersService.GetOrderByIdAndRoleAsync(id, User);
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

            var order = await ordersService.GetOrderByIdAndRoleAsync(orderStatus.OrderId, User);
            if (order == null)
                return RedirectToAction("Index", "Orders");

            if (!ModelState.IsValid)
            {
                DropdownUtil.LoadStatusDropdownData(order, ViewBag);
                return View(orderStatus);
            }

            if (orderStatus.CurrentStatus.HasValue && orderStatus.NewStatus.HasValue)
            {
                await ordersService.UpdateOrderStatusAsync(
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
