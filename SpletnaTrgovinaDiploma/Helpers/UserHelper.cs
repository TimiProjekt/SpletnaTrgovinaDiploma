using Microsoft.AspNetCore.Identity;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Helpers
{
    public class UserHelper
    {
        private const string UNREGISTERED_PASSWORD = "Lulek@123";
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserHelper(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
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
            var newUser = new ApplicationUser
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.EmailAddress
            };

            newUser.DeliveryInfo.PersonName = registerViewModel.PersonName;
            newUser.DeliveryInfo.PersonSurname = registerViewModel.PersonSurname;

            return await Register(newUser, registerViewModel.Password);
        }

        public async Task<IdentityResult> Register(UnregisteredViewModel unregisteredViewModel)
        {
            var newUser = new ApplicationUser()
            {
                Email = unregisteredViewModel.DeliveryInfo.EmailAddress,
                UserName = unregisteredViewModel.DeliveryInfo.EmailAddress,
            };

            newUser.DeliveryInfo.PersonName = unregisteredViewModel.DeliveryInfo.PersonName;
            newUser.DeliveryInfo.PersonSurname = unregisteredViewModel.DeliveryInfo.PersonSurname;
            newUser.DeliveryInfo.StreetName = unregisteredViewModel.DeliveryInfo.StreetName;
            newUser.DeliveryInfo.HouseNumber = unregisteredViewModel.DeliveryInfo.HouseNumber;
            newUser.DeliveryInfo.ZipCode = unregisteredViewModel.DeliveryInfo.ZipCode;
            newUser.DeliveryInfo.City = unregisteredViewModel.DeliveryInfo.City;
            newUser.DeliveryInfo.CountryId = unregisteredViewModel.DeliveryInfo.CountryId;
            newUser.DeliveryInfo.Country = unregisteredViewModel.DeliveryInfo.Country;

            return await Register(newUser, UNREGISTERED_PASSWORD);
        }
    }
}
