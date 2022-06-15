using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpletnaTrgovinaDiploma.Data;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AppDbContext context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
        }

        public async Task<IActionResult> Users()
        {
            var users = await context.Users.ToListAsync();
            return View(users);
        }

        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);

            var user = await userManager.FindByEmailAsync(loginViewModel.EmailAddress);
            if (user != null)
            {
                var passwordCheck = await userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                {
                    var result = await signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                        return RedirectToAction("Index", "Items");
                }

                TempData["Error"] = "Wrong credentials. Please, try again!";
                return View(loginViewModel);
            }

            TempData["Error"] = "Wrong credentials. Please, try again!";
            return View(loginViewModel);
        }

        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(registerViewModel);

            var user = await userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(registerViewModel);
            }

            var newUser = new ApplicationUser()
            {
                FullName = registerViewModel.FullName,
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.EmailAddress
            };
            var newUserResponse = await userManager.CreateAsync(newUser, registerViewModel.Password);

            if (!newUserResponse.Succeeded)
            {
                ModelState.AddModelError("", newUserResponse.Errors.First().Description);
                return View();
            }

            await userManager.AddToRoleAsync(newUser, UserRoles.User);

            return View("RegisterCompleted");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Items");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
