﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope()) 
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

                context.Database.EnsureCreated();

                //Brands
                if (!context.Brands.Any())
                {
                    context.Brands.AddRange(new List<Brand>()
                    {
                        new Brand()
                        {
                            Name = "Razer",
                            ProfilePictureURL = "https://logos-world.net/wp-content/uploads/2020/11/Razer-Logo.png"
                        },
                        new Brand()
                        {
                            Name = "Logitech",
                            ProfilePictureURL = "https://logos-world.net/wp-content/uploads/2020/11/Logitech-Logo.png"
                        },
                        new Brand()
                        {
                            Name = "Asus",
                            ProfilePictureURL = "https://logos-world.net/wp-content/uploads/2020/07/Asus-Logo.png"
                        }
                    });
                    context.SaveChanges();
                }
                //Items
                if (!context.Items.Any())
                {
                    context.Items.AddRange(new List<Item>()
                    {
                        new Item()
                        {
                            Name = "ASUS TUF Gaming H3 Wireless Headphones",
                            Description = "To je opis za slusalice",
                            Price = 100,
                            ImageURL = "https://www.mimovrste.com/i/57789139/2000/2000",
                            ItemCategory = ItemCategory.Headphones
                        },
                        new Item()
                        {
                            Name = "Razer DeathAdder V2 Gaming Mouse",
                            Description ="To je opis za misko",
                            Price = 70,
                            ImageURL = "https://www.mimovrste.com/i/45566757/2000/2000",
                            ItemCategory = ItemCategory.Mouse
                        },
                        new Item()
                        {
                            Name = "Logitech G613 Mechanic Wireless Gaming Keyboard",
                            Description = "to je opis za tipkovnico",
                            Price = 149,
                            ImageURL = "https://www.mimovrste.com/i/40008353/1000/1000",
                            ItemCategory = ItemCategory.Keyboard
                        }
                    });
                    context.SaveChanges();
                }
                //Brands & Items
                if (!context.Brands_Items.Any())
                {
                    context.Brands_Items.AddRange(new List<Brand_Item>()
                    {
                        new Brand_Item()
                        {
                            BrandId = 1,
                            ItemId = 2
                        },
                        new Brand_Item()
                        {
                            BrandId = 3,
                            ItemId = 3
                        },
                        new Brand_Item()
                        {
                            BrandId = 2,
                            ItemId = 1
                        }

                    });
                    context.SaveChanges();
                }
            }
        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {

                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                //Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                string adminUserEmail = "admin@spletnatrgovina.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if(adminUser == null)
                {
                    var newAdminUser = new ApplicationUser()
                    {
                        FullName = "Admin User",
                        UserName = "admin-user",
                        Email = adminUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newAdminUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                
                string appUserEmail = "user@spletnatrgovina.com";

                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new ApplicationUser()
                    {
                        FullName = "Application User",
                        UserName = "app-user",
                        Email = appUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newAppUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                }

            }
        }
    }
}
