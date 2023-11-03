﻿using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpletnaTrgovinaDiploma.Data.Cart;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SpletnaTrgovinaDiploma.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpletnaTrgovinaDiploma.Helpers;

namespace SpletnaTrgovinaDiploma.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ICountryService countryService;
        private readonly IItemsService itemsService;
        private readonly IOrdersService ordersService;
        private readonly ShoppingCart shoppingCart;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly UserHelper userHelper;

        public OrdersController(ICountryService countryService, IItemsService itemsService, IOrdersService ordersService, ShoppingCart shoppingCart, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.countryService = countryService;
            this.itemsService = itemsService;
            this.ordersService = ordersService;
            this.shoppingCart = shoppingCart;
            this.userManager = userManager;

            userHelper = new UserHelper(userManager, signInManager);
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var orders = await ordersService.GetOrdersByUserIdAndRoleAsync(userId, userRole);
            SetPageDetails("Orders", "Orders");
            return View(orders);
        }

        public async Task<IActionResult> Filter(string searchString)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allOrders = await ordersService.GetOrdersByUserIdAndRoleAsync(userId, userRole);

            if (!string.IsNullOrEmpty(searchString))
            {
                bool ContainsCaseInsensitiveString(string source, string contains) => source?.ToUpper().Contains(contains.ToUpper()) ?? false;

                var filteredOrders = allOrders.Where(n => ContainsCaseInsensitiveString(n.Id.ToString(), searchString) || ContainsCaseInsensitiveString(n.DeliveryEmailAddress, searchString));

                SetPageDetails("Search result", $"Search result for \"{searchString}\"");
                return View("Index", filteredOrders);
            }

            SetPageDetails("Orders", "Orders");
            return View("Index", allOrders);
        }

        void SetPageDetails(string title, string description)
        {
            ViewData["Title"] = title;
            ViewData["Description"] = description;
        }

        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var orders = await ordersService.GetOrdersByUserIdAndRoleAsync(userId, userRole);
            var order = orders.SingleOrDefault(o => o.Id == id);
            if (order == null)
                return RedirectToAction("Index", "Items");

            LoadCountriesDropdownData();
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
            var emailAddress = userManager.GetUserName(User);
            if (emailAddress != null)
            {
                var user = userManager.FindByNameAsync(emailAddress).Result;

                if (user != null)
                    return RedirectToAction(nameof(ShippingAndPayment));
            }

            var loginViewModel = new LoginViewModel();

            LoadCountriesDropdownData();

            return View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeliveryInfo(LoginViewModel loginViewModel)
        {
            LoadCountriesDropdownData();
            if (!ModelState.IsValid)
                return View(loginViewModel);

            var loginResult = await userHelper.Login(loginViewModel);
            if (loginResult.Succeeded)
                return RedirectToAction(nameof(ShippingAndPayment));

            TempData["Error"] = "Wrong credentials. Please, try again!";
            return View(loginViewModel);
        }

        ShoppingCart GetShoppingCartWithItems()
        {
            var items = shoppingCart.GetShoppingCartItems();
            shoppingCart.ShoppingCartItems = items;

            return shoppingCart;
        }

        public IActionResult ShippingAndPayment()
        {
            LoadCountriesDropdownData();
            var myShoppingCart = GetShoppingCartWithItems();

            var user = userHelper.GetApplicationUser(User);
            var response = new ShippingAndPaymentViewModel()
            {
                ShoppingCart = myShoppingCart,
                ShoppingCartTotal = myShoppingCart.GetShoppingCartTotal(),
                EmailAddress = user?.DeliveryEmailAddress ?? "",
                FullName = user?.FullName ?? "",
                PhoneNumber = user?.PhoneNumber ?? "",
                StreetName = user?.StreetName ?? "",
                HouseNumber = user?.HouseNumber ?? "",
                City = user?.City ?? "",
                ZipCode = user?.ZipCode ?? "",
                CountryId = user?.CountryId ?? 1
            };

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> ShippingAndPayment(ShippingAndPaymentViewModel shippingAndPaymentViewModel)
        {
            LoadCountriesDropdownData();
            var myShoppingCart = GetShoppingCartWithItems();
            shippingAndPaymentViewModel.ShoppingCart = myShoppingCart;
            shippingAndPaymentViewModel.ShoppingCartTotal = myShoppingCart.GetShoppingCartTotal();

            if (!ModelState.IsValid)
                return View(shippingAndPaymentViewModel);

            var items = myShoppingCart.GetShoppingCartItems();
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);

            await ordersService.StoreOrderAsync(shippingAndPaymentViewModel, items, userId);
            await myShoppingCart.ClearShoppingCartAsync();

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
