using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Helpers
{
    public class SignInHelper
    {
        private const string UNREGISTERED_PASSWORD = "Lulek@123";
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public SignInHelper(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<SignInResult> Login(LoginViewModel loginViewModel)
        {
            var user = await userManager.FindByEmailAsync(loginViewModel.EmailAddress);
            if (user != null)
            {
                var passwordCheck = await userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                    return await signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
            }

            return SignInResult.Failed;
        }

        async Task<IdentityResult> Register(ApplicationUser applicationUser, string password)
        {
            var user = await userManager.FindByEmailAsync(applicationUser.Email);
            if (user != null)
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Description = "This email address is already in use."
                });
            }

            var newUserResponse = await userManager.CreateAsync(applicationUser, password);

            if (newUserResponse.Succeeded)
            {
                await userManager.AddToRoleAsync(applicationUser, UserRoles.User);

                var loginViewModel = new LoginViewModel()
                {
                    EmailAddress = applicationUser.Email,
                    Password = password,
                };
                await Login(loginViewModel);
            }

            return newUserResponse;
        }

        public async Task<IdentityResult> Register(RegisterViewModel registerViewModel)
        {
            var newUser = new ApplicationUser()
            {
                FullName = registerViewModel.FullName,
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.EmailAddress,
                DeliveryEmailAddress = registerViewModel.EmailAddress,
            };

            return await Register(newUser, registerViewModel.Password);
        }

        public async Task<IdentityResult> Register(UserInfoViewModel unregisteredUser)
        {
            var newUser = new ApplicationUser()
            {
                FullName = unregisteredUser.FullName,
                Email = unregisteredUser.EmailAddress,
                UserName = unregisteredUser.EmailAddress,
                DeliveryEmailAddress = unregisteredUser.EmailAddress,
                PhoneNumber = unregisteredUser.PhoneNumber,
                DeliveryPhoneNumber = unregisteredUser.PhoneNumber,
                StreetName = unregisteredUser.StreetName,
                HouseNumber = unregisteredUser.HouseNumber,
                ZipCode = unregisteredUser.ZipCode,
                City = unregisteredUser.City,
                CountryId = unregisteredUser.CountryId,
                Country = unregisteredUser.Country
            };

            return await Register(newUser, UNREGISTERED_PASSWORD);
        }

        public ApplicationUser GetApplicationUser(ClaimsPrincipal userPrincipal)
        {
            var userEmailAddress = userPrincipal.FindFirstValue(ClaimTypes.Email);
            if (userEmailAddress != null)
                return userManager.FindByEmailAsync(userEmailAddress).Result;

            return null;
        }
    }
}
