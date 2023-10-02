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
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AppDbContext context;
        private readonly UserHelper userHelper;

        public OrdersController(ICountryService countryService, IItemsService itemsService, IOrdersService ordersService, ShoppingCart shoppingCart, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext context)
        {
            this.countryService = countryService;
            this.itemsService = itemsService;
            this.ordersService = ordersService;
            this.shoppingCart = shoppingCart;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;

            userHelper = new UserHelper(userManager, signInManager);
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
            var userName = userManager.GetUserName(User);
            if (userName != null)
            {
                var user = userManager.FindByNameAsync(userName).Result;

                if (user != null)
                    return RedirectToAction(nameof(ShippingAndPayment));
            }

            var deliveryInfoViewModel = new LoginOrRegisterViewModel() { IsRegistered = true };

            LoadCountriesDropdownData();

            return View(deliveryInfoViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeliveryInfo(LoginOrRegisterViewModel deliveryInfoViewModel)
        {
            LoadCountriesDropdownData();
            if (!ModelState.IsValid)
                return View(deliveryInfoViewModel);

            if (deliveryInfoViewModel.IsRegistered)
            {

                var loginResult = await userHelper.Login(deliveryInfoViewModel.RegisteredUser);
                if (loginResult.Succeeded)
                    return RedirectToAction(nameof(ShippingAndPayment));

                TempData["Error"] = "Wrong credentials. Please, try again!";
                return View(deliveryInfoViewModel);
            }

            else
            {
                var registerResult = await userHelper.Register(deliveryInfoViewModel.UnregisteredUser);
                if (registerResult.Succeeded)
                    return RedirectToAction(nameof(ShippingAndPayment));

                if (!registerResult.Succeeded)
                {
                    var error = registerResult.Errors.FirstOrDefault();
                    var errorMessage = error?.Description ?? "Unknown error.";

                    TempData["Error"] = errorMessage;
                    return View(deliveryInfoViewModel);
                }

                return RedirectToAction(nameof(ShippingAndPayment));
            }
        }

        public IActionResult ShippingAndPayment()
        {
            var items = shoppingCart.GetShoppingCartItems();
            shoppingCart.ShoppingCartItems = items;

            var userEmailAddress = User.FindFirstValue(ClaimTypes.Email);
            var user = userManager.FindByEmailAsync(userEmailAddress).Result;

            var country = context.Countries.SingleOrDefault(c => c.Id == user.DeliveryInfo.CountryId);
            var response = new ShippingAndPaymentViewModel()
            {
                ShoppingCart = shoppingCart,
                ShoppingCartTotal = shoppingCart.GetShoppingCartTotal(),
                StreetName = user.DeliveryInfo.StreetName,
                HouseNumber = user.DeliveryInfo.HouseNumber,
                City = user.DeliveryInfo.City,
                ZipCode = user.DeliveryInfo.ZipCode,
                CountryName = country?.Name,
                HasAddress = user.DeliveryInfo.HasAddress
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
