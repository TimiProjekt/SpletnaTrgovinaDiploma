using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Data
{
    public class AppDbInitializer
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                if (context == null)
                    return;

                await context.Database.EnsureCreatedAsync();

                AddBrands(context);
                AddCountries(context);
                AddItems(context);
                AddBrandsAndItems(context);

                await AddRoles(serviceScope);
                await AddUsers(serviceScope);
            }
        }

        static void AddBrands(AppDbContext context)
        {
            if (!context.Brands.Any())
            {
                context.Brands.AddRange(new List<Brand>()
                {
                    new Brand()
                    {
                        Name = "Razer",
                        ProfilePictureUrl = "https://logos-world.net/wp-content/uploads/2020/11/Razer-Logo.png"
                    },
                    new Brand()
                    {
                        Name = "Logitech",
                        ProfilePictureUrl = "https://logos-world.net/wp-content/uploads/2020/11/Logitech-Logo.png"
                    },
                    new Brand()
                    {
                        Name = "Asus",
                        ProfilePictureUrl = "https://logos-world.net/wp-content/uploads/2020/07/Asus-Logo.png"
                    }
                });
                context.SaveChanges();
            }
        }

        static void AddCountries(AppDbContext context)
        {
            if (!context.Countries.Any())
            {
                context.Countries.AddRange(new List<Country>()
                {
                    new Country()
                    {
                        Name = "Slovenia",
                    },
                    new Country()
                    {
                        Name = "Netherlands",
                    },
                    new Country()
                    {
                        Name = "United States of America",
                    }
                });
                context.SaveChanges();
            }
        }

        static void AddItems(AppDbContext context)
        {
            if (!context.Items.Any())
            {
                context.Items.AddRange(new List<Item>()
                {
                    new Item()
                    {
                        Name = "ASUS TUF Gaming H3 Wireless Headphones",
                        Description = "To je opis za slusalke",
                        ShortDescription = "To je opis za slusalke",
                        Price = 100,
                        ImageUrl = "https://www.mimovrste.com/i/57789139/2000/2000",
                        ItemCategory = ItemCategory.Headphones
                    },
                    new Item()
                    {
                        Name = "Razer DeathAdder V2 Gaming Mouse",
                        Description ="To je opis za misko",
                        ShortDescription ="To je opis za misko",
                        Price = 70,
                        ImageUrl = "https://www.mimovrste.com/i/45566757/2000/2000",
                        ItemCategory = ItemCategory.Mouse
                    },
                    new Item()
                    {
                        Name = "Logitech G613 Mechanic Wireless Gaming Keyboard",
                        Description = "to je opis za tipkovnico",
                        ShortDescription = "to je opis za tipkovnico",
                        Price = 149,
                        ImageUrl = "https://www.mimovrste.com/i/40008353/1000/1000",
                        ItemCategory = ItemCategory.Keyboard
                    }
                });
                context.SaveChanges();
            }
        }

        static void AddBrandsAndItems(AppDbContext context)
        {
            if (!context.BrandsItems.Any())
            {
                context.BrandsItems.AddRange(new List<BrandItem>()
                {
                    new BrandItem()
                    {
                        BrandId = 1,
                        ItemId = 2
                    },
                    new BrandItem()
                    {
                        BrandId = 3,
                        ItemId = 3
                    },
                    new BrandItem()
                    {
                        BrandId = 2,
                        ItemId = 1
                    }

                });
                context.SaveChanges();
            }
        }

        static async Task AddRoles(IServiceScope serviceScope)
        {
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        }

        static async Task AddUsers(IServiceScope serviceScope)
        {
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            async Task AddNewUser(string fullName, string userName, string email, string password, string userRole)
            {
                var adminUser = await userManager.FindByEmailAsync(email);
                if (adminUser == null)
                {
                    var newAdminUser = new ApplicationUser()
                    {
                        FullName = fullName,
                        UserName = userName,
                        Email = email,
                        EmailConfirmed = true,
                        CountryId = 1,
                        //Country = new Country{Id = 1, Name = "Slovenia"}
                    };
                    await userManager.CreateAsync(newAdminUser, password);
                    await userManager.AddToRoleAsync(newAdminUser, userRole);
                }
            }

            await AddNewUser(
                "Admin User",
                "admin-user",
                "admin@spletnatrgovina.com",
                "Coding@1234?",
                UserRoles.Admin);

            await AddNewUser(
                "Application User",
                "app-user",
                "user@spletnatrgovina.com",
                "Coding@1234?",
                UserRoles.User);
        }
    }
}
