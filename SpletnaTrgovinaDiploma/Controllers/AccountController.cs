using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpletnaTrgovinaDiploma.Data;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Helpers;
using X.PagedList;

namespace SpletnaTrgovinaDiploma.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AppDbContext context;
        private readonly SignInHelper signInHelper;
        private readonly ICountryService countryService;
        private readonly IOrdersService ordersService;
        private readonly INewsletterEmailService newsletterEmailService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext context,
            ICountryService countryService,
            IOrdersService ordersService,
            INewsletterEmailService newsletterEmailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            this.countryService = countryService;
            this.ordersService = ordersService;
            this.newsletterEmailService = newsletterEmailService;

            signInHelper = new SignInHelper(userManager, signInManager);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult Users(string currentFilter, string searchString, int page = 1)
        {
            var filteredUsers = GetFilteredUsers(currentFilter, searchString, page);
            return View(filteredUsers);
        }

        IPagedList<ApplicationUser> GetFilteredUsers(string currentFilter, string searchString, int page)
        {
            const int itemsPerPage = 12;

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;
            var allUsers = context.Users
                .Include(u => u.Country);

            if (!string.IsNullOrEmpty(searchString))
            {
                var upperCaseSearchString = searchString.ToUpper();
                var filteredResult = allUsers
                    .Where(n => n.FullName.ToUpper().Contains(upperCaseSearchString)
                                || n.EmailAddress.ToUpper().Contains(upperCaseSearchString));

                ViewData.SetPageDetails("Search result", $"Search result for \"{searchString}\"");
                return filteredResult.ToPagedList(page, itemsPerPage);
            }

            ViewData.SetPageDetails("Home page", "Home page of Gaming svet");
            return allUsers.ToPagedList(page, itemsPerPage);
        }

        public IActionResult Login()
            => View(new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            var loginResult = await signInHelper.Login(loginViewModel);
            if (!loginResult.Succeeded)
            {
                TempData.SetError("Wrong credentials. Please, try again!");
                return View(loginViewModel);
            }

            return RedirectToAction("Index", "Items");
        }

        public IActionResult ForgotPassword()
            => View(new ResetPasswordViewModel());

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            var email = resetPasswordViewModel.EmailAddress;
            if (string.IsNullOrEmpty(email))
            {
                TempData.SetError("Email address should not be empty.");
                return View(resetPasswordViewModel);
            }

            var appUser = await userManager.FindByNameAsync(email);
            if (appUser == null)
            {
                TempData.SetError("Email address does not exist.");
                return View(resetPasswordViewModel);
            }

            await appUser.SendForgotPasswordEmail(email, userManager);

            return View(
                "Success",
                new SuccessViewModel(
                    $"An e-mail has been sent to {email}.",
                    "Please follow the link in the e-mail to reset your password."));
        }

        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            if (email == null || token == null)
                return View("NotFound");

            var appUser = await userManager.FindByNameAsync(email);
            if (appUser == null)
                return View("NotFound");

            var registerViewModel = new ResetPasswordViewModel
            {
                EmailAddress = email,
                Token = HttpUtility.UrlDecode(token)
            };

            return View(registerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (resetPasswordViewModel.Password != resetPasswordViewModel.ConfirmPassword)
            {
                TempData.SetError("Passwords do not match!");
                return View(resetPasswordViewModel);
            }

            var appUser = await userManager.FindByNameAsync(resetPasswordViewModel.EmailAddress);
            if (appUser == null)
            {
                TempData.SetError("Internal error, please try again. If it persists, contact your administrator.");
                return View(resetPasswordViewModel);
            }

            var result = await userManager.ResetPasswordAsync(appUser, resetPasswordViewModel.Token, resetPasswordViewModel.Password);
            if (!result.Succeeded)
            {
                TempData.SetError($"Error: {result.Errors.First().Description}");
                return View(resetPasswordViewModel);
            }

            return View(
                "Success",
                new SuccessViewModel(
                    "Password reset success.",
                    "You can now login."));
        }

        public IActionResult Register()
            => View(new RegisterViewModel());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
                return View(registerViewModel);

            var registerResult = await signInHelper.Register(registerViewModel);
            if (!registerResult.Succeeded)
            {
                var error = registerResult.Errors.FirstOrDefault();
                var errorMessage = error?.Description ?? "Unknown error.";

                TempData.SetError(errorMessage);
                return View(registerViewModel);
            }

            registerViewModel.SendRegisterConfirmationEmail();
            return View("RegisterCompleted");
        }

        [HttpPost]
        public async Task<IActionResult> SubscribeToNewsletter(string email)
        {
            var emailValidationSuccess = EmailProvider.IsValidEmail(email);
            if (!emailValidationSuccess)
                return View("Failure", new SuccessViewModel("Entered e-mail is invalid.", ""));

            var result = await newsletterEmailService.AddToMailingListAsync(email);
            if (!result)
                return View("Failure", new SuccessViewModel("Email already subscribed to newsletter.", ""));

            EmailSenderUtil.SendNewsletterSubscriptionConfirmationEmail(email);
            return View("Success", new SuccessViewModel("Successfully registered for the newsletter.", ""));
        }

        public async Task<IActionResult> UnsubscribeFromNewsletter(string email)
        {
            var emailValidationSuccess = EmailProvider.IsValidEmail(email);
            if (!emailValidationSuccess)
                return View("Failure", new SuccessViewModel("Entered e-mail is invalid.", ""));

            var result = await newsletterEmailService.RemoveFromMailingListAsync(email);
            if (!result)
                return View("Failure", new SuccessViewModel("Email not subscribed to our newsletter.", ""));

            EmailSenderUtil.SendNewsletterUnsubscribeConfirmationEmail(email);
            return View("Success", new SuccessViewModel("Successfully unsubscribed from the newsletter.", ""));
        }

        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult SubscriberEmail()
            => View(new SuccessViewModel("", ""));

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult SubscriberEmail(SuccessViewModel successViewModel)
        {
            var allEmailAddresses = newsletterEmailService.AllEmailAddressesFromMailingList;
            var lastFiveEmailAddresses = allEmailAddresses.TakeLast(5).ToList();

            foreach (var newsletterEmail in lastFiveEmailAddresses)
            {
                EmailProvider.SendEmail(
                    newsletterEmail.Email,
                    successViewModel.Header,
                    successViewModel.Body);
            }

            return View("Success", new SuccessViewModel($"Emails successfully sent to all {lastFiveEmailAddresses.Count} subscribers.", ""));
        }

        [Authorize]
        public IActionResult Settings()
        {
            var appUser = User.GetApplicationUser(userManager);
            if (appUser == null)
            {
                TempData.SetError("Cannot fetch settings or email.");
                return View(new UserInfoViewModel());
            }

            var settingsViewModel = new UserInfoViewModel
            {
                EmailAddress = appUser.EmailAddress,
                FullName = appUser.FullName,
                PhoneNumber = appUser.PhoneNumber,
                StreetName = appUser.StreetName,
                HouseNumber = appUser.HouseNumber,
                City = appUser.City,
                ZipCode = appUser.ZipCode,
                CountryId = appUser.CountryId
            };

            DropdownUtil.LoadCountriesDropdownData(countryService, ViewBag);
            return View(settingsViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Settings(UserInfoViewModel userInfoViewModel)
        {
            if (!ModelState.IsValid)
                return View(userInfoViewModel);

            if (userInfoViewModel.EmailAddress == null)
            {
                TempData.SetError("Cannot fetch settings or email");
                return View();
            }

            var appUser = userManager.GetApplicationUserFromEmail(userInfoViewModel.EmailAddress);
            if (appUser == null)
            {
                TempData.SetError("Cannot fetch settings or email");
                return View();
            }

            appUser.FullName = userInfoViewModel.FullName;
            appUser.PhoneNumber = userInfoViewModel.PhoneNumber;
            appUser.DeliveryPhoneNumber = userInfoViewModel.PhoneNumber;
            appUser.StreetName = userInfoViewModel.StreetName;
            appUser.HouseNumber = userInfoViewModel.HouseNumber;
            appUser.City = userInfoViewModel.City;
            appUser.ZipCode = userInfoViewModel.ZipCode;
            appUser.Country = context.Countries.Single(c => c.Id == userInfoViewModel.CountryId);

            var updateUserResponse = await userManager.UpdateAsync(appUser);
            if (!updateUserResponse.Succeeded)
            {
                ModelState.AddModelError("", updateUserResponse.Errors.First().Description);
                return View();
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Items");
        }

        public IActionResult AccessDenied()
            => View();

        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult GetUserInfo(string userId)
        {
            IDeliveryInfo deliveryInfo = userManager.FindByIdAsync(userId).Result;
            if (deliveryInfo == null)
            {
                TempData.SetError("Cannot fetch user information.");
                return View(new UserInfoViewModel());
            }

            var userInfoViewModel = deliveryInfo.CreateInfoViewModel();
            DropdownUtil.LoadCountriesDropdownData(countryService, ViewBag);
            return View(userInfoViewModel);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetUnregisteredUserInfo(int orderId)
        {
            IDeliveryInfo deliveryInfo = await ordersService.GetOrderByIdAndRoleAsync(orderId, User);
            if (deliveryInfo == null)
            {
                TempData.SetError("Cannot fetch user information.");
                return View(new UserInfoViewModel());
            }

            var userInfoViewModel = deliveryInfo.CreateInfoViewModel();
            DropdownUtil.LoadCountriesDropdownData(countryService, ViewBag);
            return View(userInfoViewModel);
        }
    }
}
