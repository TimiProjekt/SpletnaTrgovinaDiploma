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
            var newUser = new ApplicationUser()
            {
                FullName = registerViewModel.FullName,
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.EmailAddress
            };

            return await Register(newUser, registerViewModel.Password);
        }

        public async Task<IdentityResult> Register(UnregisteredViewModel unregisteredViewModel)
        {
            var newUser = new ApplicationUser()
            {
                FullName = unregisteredViewModel.PersonName + " " + unregisteredViewModel.PersonSurname,
                Email = unregisteredViewModel.EmailAddress,
                UserName = unregisteredViewModel.EmailAddress,
                StreetName = unregisteredViewModel.StreetName,
                HouseNumber = unregisteredViewModel.HouseNumber,
                ZipCode = unregisteredViewModel.ZipCode,
                City = unregisteredViewModel.City,
                CountryId = unregisteredViewModel.CountryId,
                Country = unregisteredViewModel.Country
            };

            return await Register(newUser, UNREGISTERED_PASSWORD);
        }
    }
}
