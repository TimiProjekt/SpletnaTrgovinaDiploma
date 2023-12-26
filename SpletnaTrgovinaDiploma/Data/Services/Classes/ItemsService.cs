using System.Collections.Generic;
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
                Price = data.Price ?? 0,
                ImageUrl = data.ImageUrl,
                ItemCategory = data.ItemCategory ?? ItemCategory.Unknown,
                Descriptions = data.ItemDescriptions,
                BrandsItems = new List<BrandItem>()
            };

            AssignBrandIdsFromBrandNames(newItem, data, context.Brands.ToList());
            context.Items.Add(newItem);

            await context.SaveChangesAsync();
        }

        public async Task AddNewItemsAsync(List<NewItemViewModel> data)
        {
            var newItems = new List<Item>();
            var newItemDescriptions = new List<ItemDescription>();
            var allBrands = context.Brands.ToList();

            foreach (var newItemViewModel in data)
            {
                var newItem = new Item()
                {
                    Name = newItemViewModel.Name,
                    Description = newItemViewModel.Description,
                    ShortDescription = newItemViewModel.ShortDescription,
                    Price = newItemViewModel.Price ?? 0,
                    ImageUrl = newItemViewModel.ImageUrl,
                    ItemCategory = newItemViewModel.ItemCategory ?? ItemCategory.Unknown,
                    ProductCode = newItemViewModel.ProductCode,
                    Availability = newItemViewModel.Availability,
                    Descriptions = newItemViewModel.ItemDescriptions,
                    BrandsItems = new List<BrandItem>()
                };
                newItems.Add(newItem);

                foreach (var itemDescription in newItem.Descriptions)
                {
                    var newItemDescription = new ItemDescription()
                    {
                        ItemId = newItem.Id,
                        Name = itemDescription.Name,
                        Description = itemDescription.Description,
                    };

                    newItemDescriptions.Add(newItemDescription);
                }

                AssignBrandIdsFromBrandNames(newItem, newItemViewModel, allBrands);
            }

            context.ItemDescriptions.AddRange(newItemDescriptions);
            context.Items.AddRange(newItems);
            await context.SaveChangesAsync();
        }


        public async Task<Item> GetItemByIdAsync(int id)
        {
            var itemDetails = await context.Items
                .Include(am => am.Descriptions)
                .Include(am => am.BrandsItems)
                .ThenInclude(b => b.Brand)
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
                dbItem.Price = data.Price ?? 0;
                dbItem.ImageUrl = data.ImageUrl;
                dbItem.ItemCategory = data.ItemCategory ?? ItemCategory.Unknown;
                dbItem.Descriptions = data.ItemDescriptions;
                dbItem.ProductCode = data.ProductCode;
                dbItem.Availability = data.Availability;
                await context.SaveChangesAsync();
            }

            //Remove existing items
            var existingBrandsDb = context.BrandsItems.Where(n => n.ItemId == data.Id).ToList();
            context.BrandsItems.RemoveRange(existingBrandsDb);
            await context.SaveChangesAsync();

            AssignBrandIdsFromBrandNames(dbItem, data, context.Brands.ToList());
            await context.SaveChangesAsync();
        }

        public async Task<int> UpdateItemsNonNullValuesAsync(List<NewItemViewModel> data)
        {
            var amountOfUpdatedItems = 0;
            var allItems = context.Items.ToList();
            var allBrands = context.Brands.ToList();

            foreach (var item in data)
            {
                var dbItem = allItems.FirstOrDefault(n => n.ProductCode == item.ProductCode);

                if (!string.IsNullOrEmpty(item.ProductCode) && dbItem != null)
                {
                    amountOfUpdatedItems++;

                    //only if item found, it could be update, otherwise create a new one
                    dbItem.Name = item.Name ?? dbItem.Name;
                    dbItem.Description = item.Description ?? dbItem.Description;
                    dbItem.ShortDescription = item.ShortDescription ?? dbItem.ShortDescription;
                    dbItem.Price = item.Price ?? dbItem.Price;
                    dbItem.ImageUrl = item.ImageUrl ?? dbItem.ImageUrl;
                    dbItem.ItemCategory = item.ItemCategory ?? dbItem.ItemCategory;
                    dbItem.ProductCode = item.ProductCode ?? dbItem.ProductCode;
                    dbItem.Availability = item.Availability ?? dbItem.Availability;
                }

                AssignBrandIdsFromBrandNames(dbItem, item, allBrands);
            }

            await context.SaveChangesAsync();

            return amountOfUpdatedItems;
        }

        void AssignBrandIdsFromBrandNames(Item newItem, NewItemViewModel newItemViewModel, List<Brand> allBrands)
        {
            // New Brands that do not exist yet
            foreach (var brandName in newItemViewModel.BrandNames)
            {
                var findBrand = allBrands.FirstOrDefault(b => b.Name == brandName);

                if (findBrand == null)
                {
                    // insert brand
                    var brand = new Brand()
                    {
                        Name = brandName,
                        ProfilePictureUrl = ""
                    };

                    context.Brands.Add(brand);
                    allBrands.Add(brand);

                    findBrand = brand;
                }
                if (newItem.BrandsItems.All(item => item.Brand.Name != brandName))
                {
                    var newBrandItem = new BrandItem()
                    {
                        Item = newItem,
                        Brand = findBrand
                    };

                    newItem.BrandsItems.Add(newBrandItem);
                }
            }

            foreach (var brandId in newItemViewModel.BrandIds)
            {
                if (newItem.BrandsItems.All(item => item.BrandId != brandId))
                {
                    var newBrandItem = new BrandItem()
                    {
                        Item = newItem,
                        BrandId = brandId
                    };
                    newItem.BrandsItems.Add(newBrandItem);
                }
            }

        }
    }
}
