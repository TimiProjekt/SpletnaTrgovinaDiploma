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
                user.Country = context.Countries.SingleOrDefault(c => c.Id == user.CountryId);

            return View(users);
        }

        public async Task<IActionResult> Filter(string searchString)
        {
            var users = await context.Users.ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                var upperCaseSearchString = searchString.ToUpper();
                var filteredResult = users
                    .Where(n => n.FullName.ToUpper().Contains(upperCaseSearchString) || n.EmailAddress.ToUpper().Contains(upperCaseSearchString));

                SetPageDetails("Search result", $"Search result for \"{searchString}\"");
                return View("Users", filteredResult);
            }

            SetPageDetails("Users", "Search result");
            return View("Users", users);
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
            var emailAddress = userManager.GetUserName(User);
            var user = userManager.FindByNameAsync(emailAddress).Result;

            if (user != null)
            {
                var settingsViewModel = new UserInfoViewModel
                {
                    EmailAddress = emailAddress,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    StreetName = user.StreetName,
                    HouseNumber = user.HouseNumber,
                    City = user.City,
                    ZipCode = user.ZipCode,
                    CountryId = user.CountryId
                };

                LoadCountriesDropdownData();
                return View(settingsViewModel);
            }

            TempData["Error"] = "Cannot fetch settings or email.";

            return View(new UserInfoViewModel());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Settings(UserInfoViewModel userInfoViewModel)
        {
            LoadCountriesDropdownData();
            if (!ModelState.IsValid)
                return View(userInfoViewModel);

            if (userInfoViewModel.EmailAddress != null)
            {
                var user = await userManager.FindByNameAsync(userInfoViewModel.EmailAddress);
                user.FullName = userInfoViewModel.FullName;
                user.PhoneNumber = userInfoViewModel.PhoneNumber;
                user.DeliveryPhoneNumber = userInfoViewModel.PhoneNumber;
                user.StreetName = userInfoViewModel.StreetName;
                user.HouseNumber = userInfoViewModel.HouseNumber;
                user.City = userInfoViewModel.City;
                user.ZipCode = userInfoViewModel.ZipCode;
                user.Country = context.Countries.Single(c => c.Id == userInfoViewModel.CountryId);

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

        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult GetUserInfo(string userId)
        {
            var user = userManager.FindByIdAsync(userId).Result;

            if (user != null)
            {
                var userInfoViewModel = new UserInfoViewModel
                {
                    FullName = user.FullName,
                    EmailAddress = user.DeliveryEmailAddress,
                    PhoneNumber = user.DeliveryPhoneNumber,
                    StreetName = user.StreetName,
                    HouseNumber = user.HouseNumber,
                    City = user.City,
                    ZipCode = user.ZipCode,
                    CountryId = user.CountryId
                };

                LoadCountriesDropdownData();
                return View(userInfoViewModel);
            }

            TempData["Error"] = "Cannot fetch user info";

            return View(new UserInfoViewModel());
        }

        void SetPageDetails(string title, string description)
        {
            ViewData["Title"] = title;
            ViewData["Description"] = description;
        }
    }
}
