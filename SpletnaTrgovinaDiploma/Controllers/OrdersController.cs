using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpletnaTrgovinaDiploma.Data.Cart;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SpletnaTrgovinaDiploma.Data;
using SpletnaTrgovinaDiploma.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SpletnaTrgovinaDiploma.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ICountryService countryService;
        private readonly IItemsService itemsService;
        private readonly IOrdersService ordersService;
        private readonly ShoppingCart shoppingCart;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AppDbContext context;

        public OrdersController(ICountryService countryService, IItemsService itemsService, IOrdersService ordersService, ShoppingCart shoppingCart, UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            this.countryService = countryService;
            this.itemsService = itemsService;
            this.ordersService = ordersService;
            this.shoppingCart = shoppingCart;
            this.userManager = userManager;
            this.context = context;
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

            var userEmailAddress = User.FindFirstValue(ClaimTypes.Email);
            var user = userManager.FindByEmailAsync(userEmailAddress).Result;
            if (!user.HasAddress)
                TempData["Error"] = "You do not have any address entered. Please go to settings and set up an address!";

            var country = context.Countries.SingleOrDefault(c => c.Id == user.CountryId);
            var response = new ShoppingCartViewModel()
            {
                ShoppingCart = shoppingCart,
                ShoppingCartTotal = shoppingCart.GetShoppingCartTotal(),
                StreetName = user.StreetName,
                HouseNumber = user.HouseNumber,
                City = user.City,
                ZipCode = user.ZipCode,
                CountryName = country?.Name,
                HasAddress = user.HasAddress
            };

            return View(response);

        }

        [AllowAnonymous]
        public async Task<IActionResult> AddItemToShoppingCart(int id)
        {
            var item = await itemsService.GetItemByIdAsync(id);

            if (item != null && !shoppingCart.IsItemInCart(item))
                shoppingCart.AddItemToCart(item);

            return View(item);
        }

        public async Task<IActionResult> IncreaseItemInShoppingCart(int id)
        {
            var item = await itemsService.GetItemByIdAsync(id);

            if (item != null)
                shoppingCart.IncreaseItemInCart(item);

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
            var deliveryInfoViewModel = new DeliveryInfo
            {
                PersonName = "",
                PersonSurname = "",
                EmailAddress = "",
                TelephoneNumber = "",
                StreetName = "",
                HouseNumber = "",
                City = "",
                ZipCode = "",
                CountryId = null
            };
            LoadCountriesDropdownData();

            return View(deliveryInfoViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserCheckout(SettingsViewModel settingsViewModel)
        {
            LoadCountriesDropdownData();
            if (!ModelState.IsValid)
                return View(settingsViewModel);

            if (settingsViewModel.UserName != null)
            {
                var user = await userManager.FindByNameAsync(settingsViewModel.UserName);
                user.StreetName = settingsViewModel.StreetName;
                user.HouseNumber = settingsViewModel.HouseNumber;
                user.City = settingsViewModel.City;
                user.ZipCode = settingsViewModel.ZipCode;
                user.Country = context.Countries.Single(c => c.Id == settingsViewModel.CountryId);

                var updateUserResponse = await userManager.UpdateAsync(user);
                if (!updateUserResponse.Succeeded)
                {
                    ModelState.AddModelError("", updateUserResponse.Errors.First().Description);
                    return View();
                }

                return View();
            }

            TempData["Error"] = "Cannot fetch settings or email";
            return View();
        }

        public IActionResult ShippingAndPayment()
        {
            var items = shoppingCart.GetShoppingCartItems();
            shoppingCart.ShoppingCartItems = items;

            var userEmailAddress = User.FindFirstValue(ClaimTypes.Email);
            var user = userManager.FindByEmailAsync(userEmailAddress).Result;

            var country = context.Countries.SingleOrDefault(c => c.Id == user.CountryId);
            var response = new ShoppingCartViewModel()
            {
                ShoppingCart = shoppingCart,
                ShoppingCartTotal = shoppingCart.GetShoppingCartTotal(),
                StreetName = user.StreetName,
                HouseNumber = user.HouseNumber,
                City = user.City,
                ZipCode = user.ZipCode,
                CountryName = country?.Name,
                HasAddress = user.HasAddress
            };

            return View(response);
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

        void LoadCountriesDropdownData()
        {
            var defaultEmptyValue = new Country { Id = 0, Name = "-- Select a country --" };
            var itemDropdownsData = countryService.GetDropdownValuesAsync().Result;
            itemDropdownsData.Countries.Insert(0, defaultEmptyValue);
            ViewBag.Countries = new SelectList(itemDropdownsData.Countries, "Id", "Name");
        }
    }
}
