using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BrandItem>().HasKey(am => new
            {
                am.BrandId,
                am.ItemId
            });

            modelBuilder.Entity<BrandItem>().HasOne(m => m.Item).WithMany(am => am.BrandsItems).HasForeignKey(m => m.ItemId);
            modelBuilder.Entity<BrandItem>().HasOne(m => m.Brand).WithMany(am => am.BrandsItems).HasForeignKey(m => m.BrandId);


            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<DeliveryInfo> DeliveryInfos { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<BrandItem> BrandsItems { get; set; }


        //Orders related tables
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}
