using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpletnaTrgovinaDiploma.Data;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Data.Services.Classes;
using SpletnaTrgovinaDiploma.Helpers;

namespace SpletnaTrgovinaDiploma.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AppDbContext context;
        private readonly UserHelper userHelper;
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

        public IActionResult ForgotPassword() => View(new ResetPasswordViewModel());

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            var email = resetPasswordViewModel.EmailAddress;
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Email address should not be empty.";
                return View(resetPasswordViewModel);
            }

            var user = await userManager.FindByNameAsync(email);
            if (user == null)
            {
                TempData["Error"] = "Email does not exist.";
                return View(resetPasswordViewModel);
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"{ConfigurationService.PublishedUrl}/Account/ResetPassword?email={email}&token={HttpUtility.UrlEncode(token)}";

            EmailProvider.SendEmail(
                email,
                "Reset password link",
                $"Dear {user.FullName}, <br />" +
                "You requested to reset your password." +
                $"If you requested this then follow this <a href=\"{callbackUrl}\"> link </a> to reset your password." +
                "If you did not request this, then we suggest that you immediately change your password."
            );

            return View(
                "Success",
                new EmailViewModel(
                    $"An e-mail has been sent to {email}.",
                    "Please follow the link in the e-mail to reset your password."));
        }

        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            if (email == null || token == null)
                return View("NotFound");

            var user = await userManager.FindByNameAsync(email);

            if (user == null)
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
                TempData["Error"] = "Passwords do not match!";
                return View(resetPasswordViewModel);
            }

            var user = await userManager.FindByNameAsync(resetPasswordViewModel.EmailAddress);

            if (user == null)
            {
                TempData["Error"] = "Internal error, please try again. If it persists, contact your administrator.";
                return View(resetPasswordViewModel);
            }

            var result = await userManager.ResetPasswordAsync(user, resetPasswordViewModel.Token, resetPasswordViewModel.Password);

            if (!result.Succeeded)
            {
                TempData["Error"] = "Error: " + result.Errors.First().Description;
                return View(resetPasswordViewModel);
            }

            return View(
                "Success",
                new EmailViewModel(
                    "Password reset success.",
                    "You can now login."));
        }

        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            void SendConfirmationEmail()
            {
                var messageHtml = $"Hello {registerViewModel.FullName}! <br/>";
                messageHtml += "You have successfully registered at Gaming svet <br/> ";
                messageHtml += $"You can now <a href='{ConfigurationService.PublishedUrl}'> login! </a>";

                EmailProvider.SendEmail(
                    registerViewModel.EmailAddress,
                    "Your registration is completed",
                    messageHtml);
            }

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

            SendConfirmationEmail();

            return View("RegisterCompleted");
        }

        [HttpPost]
        public async Task<IActionResult> SubscribeToNewsletter(string email)
        {
            void SendConfirmationEmail()
            {
                var messageHtml = $"Hello {email}! <br/>";
                messageHtml += "You have successfully subscribed to our newsletter at Gaming svet <br/> ";

                EmailProvider.SendEmail(
                    email,
                    "Your newsletter subscription",
                    messageHtml);
            }

            var result = await newsletterEmailService.AddToMailingList(email);
            if (!result)
                return View("Failure", new EmailViewModel("Email already subscribed to newsletter.", ""));

            SendConfirmationEmail();
            return View("Success", new EmailViewModel("Successfully registered for the newsletter.", ""));
        }

        public async Task<IActionResult> UnsubscribeFromNewsletter(string email)
        {
            void SendConfirmationEmail()
            {
                var messageHtml = $"Hello {email}! <br/>";
                messageHtml += "You have successfully unsubscribed from our newsletter at Gaming svet <br/> ";

                EmailProvider.SendEmail(
                    email,
                    "Your newsletter subscription",
                    messageHtml);
            }

            var result = await newsletterEmailService.RemoveFromMailingList(email);
            if (!result)
                return View("Failure", new EmailViewModel("Email not subscribed to our newsletter.", ""));

            SendConfirmationEmail();
            return View("Success", new EmailViewModel("Successfully registered for the newsletter.", ""));
        }

        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult SubscriberEmail() => View(new EmailViewModel("", ""));

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult SubscriberEmail(EmailViewModel emailViewModel)
        {
            var allEmailAddresses = newsletterEmailService.GetAllEmailAddressesFromMailingList();

            var emailSent = 0;
            foreach (var newsletterEmail in allEmailAddresses.TakeLast(5))
            {
                EmailProvider.SendEmail(
                    newsletterEmail.Email,
                    emailViewModel.Header,
                    emailViewModel.Body);
                emailSent++;
            }

            return View("Success", new EmailViewModel($"Emails successfully sent to all {emailSent} subscribers.", ""));
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

        UserInfoViewModel CreateInfoViewModel(IDeliveryInfo deliveryInfo)
        {
            return new UserInfoViewModel
            {
                FullName = deliveryInfo.FullName,
                EmailAddress = deliveryInfo.DeliveryEmailAddress,
                PhoneNumber = deliveryInfo.DeliveryPhoneNumber,
                StreetName = deliveryInfo.StreetName,
                HouseNumber = deliveryInfo.HouseNumber,
                City = deliveryInfo.City,
                ZipCode = deliveryInfo.ZipCode,
                CountryId = deliveryInfo.CountryId
            };
        }

        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult GetUserInfo(string userId)
        {
            var user = userManager.FindByIdAsync(userId).Result;

            if (user != null)
            {
                var userInfoViewModel = CreateInfoViewModel(user);

                LoadCountriesDropdownData();
                return View(userInfoViewModel);
            }

            TempData["Error"] = "Cannot fetch user info";

            return View(new UserInfoViewModel());
        }

        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult GetUnregisteredUserInfo(int orderId)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var order = ordersService.GetOrderByIdAndRole(orderId, userRole);

            if (order != null)
            {
                var userInfoViewModel = CreateInfoViewModel(order);

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
