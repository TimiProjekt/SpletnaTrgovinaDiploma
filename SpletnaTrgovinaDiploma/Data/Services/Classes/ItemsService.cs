using Microsoft.EntityFrameworkCore;
using SpletnaTrgovinaDiploma.Data.Base;
using SpletnaTrgovinaDiploma.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public class ItemsService : EntityBaseRepository<Item>, IItemsService
    {
        private readonly AppDbContext context;

        public ItemsService(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task AddNewItemAsync(NewItemViewModel data)
        {
            var newItem = new Item()
            {
                Name = data.Name,
                Description = data.Description,
                ShortDescription = data.ShortDescription,
                Price = data.Price,
                ImageUrl = data.ImageUrl,
                ItemCategory = data.ItemCategory
            };
            await context.Items.AddAsync(newItem);
            await context.SaveChangesAsync();

            //Add Item Brands
            foreach (var brandId in data.BrandIds)
            {
                var newBrandItem = new BrandItem()
                {
                    ItemId = newItem.Id,
                    BrandId = brandId
                };
                await context.BrandsItems.AddAsync(newBrandItem);
            }
            await context.SaveChangesAsync();
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            var itemDetails = await context.Items
                .Include(am => am.BrandsItems).ThenInclude(b => b.Brand)
                .FirstOrDefaultAsync(n => n.Id == id);

            return itemDetails;
        }

        public async Task UpdateItemAsync(NewItemViewModel data)
        {
            var dbItem = await context.Items.FirstOrDefaultAsync(n => n.Id == data.Id);

            if (dbItem != null)
            {
                dbItem.Name = data.Name;
                dbItem.Description = data.Description;
                dbItem.ShortDescription = data.ShortDescription;
                dbItem.Price = data.Price;
                dbItem.ImageUrl = data.ImageUrl;
                dbItem.ItemCategory = data.ItemCategory;
                await context.SaveChangesAsync();
            }

            //Remove existing items
            var existingBrandsDb = context.BrandsItems.Where(n => n.ItemId == data.Id).ToList();
            context.BrandsItems.RemoveRange(existingBrandsDb);
            await context.SaveChangesAsync();

            //Add Item Brands
            foreach (var brandId in data.BrandIds)
            {
                var newBrandItem = new BrandItem()
                {
                    ItemId = data.Id,
                    BrandId = brandId
                };
                await context.BrandsItems.AddAsync(newBrandItem);
            }
            await context.SaveChangesAsync();
        }
    }
}
