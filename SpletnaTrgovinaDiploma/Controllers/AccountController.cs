using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpletnaTrgovinaDiploma.Data;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Helpers;

namespace SpletnaTrgovinaDiploma.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICountryService countryService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AppDbContext context;
        private readonly UserHelper userHelper;

        public AccountController(ICountryService countryService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext context)
        {
            this.countryService = countryService;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;

            userHelper = new UserHelper(userManager, signInManager);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Users()
        {
            var users = await context.Users.ToListAsync();

            foreach (var user in users)
                user.DeliveryInfo.Country = context.Countries.SingleOrDefault(c => c.Id == user.DeliveryInfo.CountryId);

            return View(users);
        }

        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            var loginResult = await userHelper.Login(loginViewModel);
            if (loginResult.Succeeded)
                return RedirectToAction("Index", "Items");

            TempData["Error"] = "Wrong credentials. Please, try again!";
            return View(loginViewModel);
        }

        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
                return View(registerViewModel);

            var registerResult = await userHelper.Register(registerViewModel);
            if (!registerResult.Succeeded)
            {
                var error = registerResult.Errors.FirstOrDefault();
                var errorMessage = error?.Description ?? "Unknown error.";

                TempData["Error"] = errorMessage;
                return View(registerViewModel);
            }

            return View("RegisterCompleted");
        }

        [Authorize]
        public IActionResult Settings()
        {
            var userName = userManager.GetUserName(User);
            var user = userManager.FindByNameAsync(userName).Result;

            if (user != null)
            {
                var settingsViewModel = new SettingsViewModel
                {
                    UserName = userName,
                    StreetName = user.DeliveryInfo.StreetName,
                    HouseNumber = user.DeliveryInfo.HouseNumber,
                    City = user.DeliveryInfo.City,
                    ZipCode = user.DeliveryInfo.ZipCode,
                    CountryId = user.DeliveryInfo.CountryId
                };

                LoadCountriesDropdownData();
                return View(settingsViewModel);
            }

            TempData["Error"] = "Cannot fetch settings or email.";

            return View(new SettingsViewModel());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Settings(SettingsViewModel settingsViewModel)
        {
            LoadCountriesDropdownData();
            if (!ModelState.IsValid)
                return View(settingsViewModel);

            if (settingsViewModel.UserName != null)
            {
                var user = await userManager.FindByNameAsync(settingsViewModel.UserName);
                user.DeliveryInfo.StreetName = settingsViewModel.StreetName;
                user.DeliveryInfo.HouseNumber = settingsViewModel.HouseNumber;
                user.DeliveryInfo.City = settingsViewModel.City;
                user.DeliveryInfo.ZipCode = settingsViewModel.ZipCode;
                user.DeliveryInfo.Country = context.Countries.Single(c => c.Id == settingsViewModel.CountryId);

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

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Items");
        }

        public IActionResult AccessDenied()
        {
            return View();
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
