using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpletnaTrgovinaDiploma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand_Item>().HasKey(am => new
            {
                am.BrandId,
                am.ItemId
            });

            modelBuilder.Entity<Brand_Item>().HasOne(m => m.Item).WithMany(am => am.Brands_Items).HasForeignKey(m => m.ItemId);
            modelBuilder.Entity<Brand_Item>().HasOne(m => m.Brand).WithMany(am => am.Brands_Items).HasForeignKey(m => m.BrandId);


            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Brand_Item> Brands_Items { get; set; }


        //Orders related tables
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}
