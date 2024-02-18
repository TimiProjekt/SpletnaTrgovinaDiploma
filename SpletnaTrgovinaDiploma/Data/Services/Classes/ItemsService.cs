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

        private IQueryable<Item> Items
            => context.Items
                .Include(am => am.Descriptions)
                .Include(am => am.BrandsItems)
                .ThenInclude(b => b.Brand);

        public ItemsService(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task AddNewItemAsync(NewItemViewModel dataItem)
        {
            var dbItem = new Item
            {
                Name = dataItem.Name,
                Description = dataItem.Description,
                ShortDescription = dataItem.ShortDescription,
                Price = dataItem.Price ?? 0,
                ImageUrl = dataItem.ImageUrl,
                ItemCategory = dataItem.ItemCategory ?? ItemCategory.Unknown,
                Descriptions = dataItem.ItemDescriptions,
                BrandsItems = new List<BrandItem>()
            };

            AssignBrandIdsFromBrandNames(dbItem, dataItem, context.Brands.ToList());

            await context.Items.AddAsync(dbItem);
            await context.SaveChangesAsync();
        }

        public async Task AddNewItemsAsync(List<NewItemViewModel> dataList)
        {
            var newItems = new List<Item>();
            var allBrands = await context.Brands.ToListAsync();

            foreach (var dataItem in dataList)
            {
                var dbItem = new Item
                {
                    Name = dataItem.Name,
                    Description = dataItem.Description,
                    ShortDescription = dataItem.ShortDescription,
                    Price = dataItem.Price ?? 0,
                    ImageUrl = dataItem.ImageUrl,
                    ItemCategory = dataItem.ItemCategory ?? ItemCategory.Unknown,
                    ProductCode = dataItem.ProductCode,
                    Availability = dataItem.Availability,
                    Descriptions = dataItem.ItemDescriptions,
                    BrandsItems = new List<BrandItem>()
                };

                AssignBrandIdsFromBrandNames(dbItem, dataItem, allBrands);
                newItems.Add(dbItem);
            }

            await context.Items.AddRangeAsync(newItems);
            await context.SaveChangesAsync();
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            var itemDetails = await Items
                .FirstOrDefaultAsync(n => n.Id == id);

            return itemDetails;
        }

        public async Task UpdateItemAsync(NewItemViewModel dataItem)
        {
            var dbItem = await Items
                .FirstOrDefaultAsync(n => n.Id == dataItem.Id);

            if (dbItem != null)
            {
                dbItem.Name = dataItem.Name;
                dbItem.Description = dataItem.Description;
                dbItem.ShortDescription = dataItem.ShortDescription;
                dbItem.Price = dataItem.Price ?? 0;
                dbItem.ImageUrl = dataItem.ImageUrl;
                dbItem.ItemCategory = dataItem.ItemCategory ?? ItemCategory.Unknown;
                dbItem.ProductCode = dataItem.ProductCode;
                dbItem.Availability = dataItem.Availability;

                FindOrCreateItemDescriptions(dbItem, dataItem.ItemDescriptions);
                AssignBrandIdsFromBrandNames(dbItem, dataItem, context.Brands.ToList());
            }

            await context.SaveChangesAsync();
        }

        public async Task<int> UpdateItemsNonNullValuesAsync(List<NewItemViewModel> dataList)
        {
            var amountOfUpdatedItems = 0;
            var allBrands = context.Brands.ToList();

            foreach (var dataItem in dataList)
            {
                var dbItem = await Items
                    .FirstOrDefaultAsync(n => n.ProductCode == dataItem.ProductCode);

                if (dbItem != null && !string.IsNullOrEmpty(dataItem.ProductCode))
                {
                    //only if item found, it could be updated, otherwise ignore
                    amountOfUpdatedItems++;

                    dbItem.Name = dataItem.Name ?? dbItem.Name;
                    dbItem.Description = dataItem.Description ?? dbItem.Description;
                    dbItem.ShortDescription = dataItem.ShortDescription ?? dbItem.ShortDescription;
                    dbItem.Price = dataItem.Price ?? dbItem.Price;
                    dbItem.ImageUrl = dataItem.ImageUrl ?? dbItem.ImageUrl;
                    dbItem.ItemCategory = dataItem.ItemCategory ?? dbItem.ItemCategory;
                    dbItem.ProductCode = dataItem.ProductCode ?? dbItem.ProductCode;
                    dbItem.Availability = dataItem.Availability ?? dbItem.Availability;

                    FindOrCreateItemDescriptions(dbItem, dataItem.ItemDescriptions);
                    AssignBrandIdsFromBrandNames(dbItem, dataItem, allBrands);
                }
            }

            await context.SaveChangesAsync();

            return amountOfUpdatedItems;
        }

        static void FindOrCreateItemDescriptions(Item item, List<ItemDescription> itemDescriptions, bool deleteOld = false)
        {
            if (deleteOld)
                item.Descriptions.Clear();

            // find or create attribute
            foreach (var itemDescription in itemDescriptions)
            {
                var findItemDescription = item.Descriptions
                    .FirstOrDefault(d => d.Name == itemDescription.Name);

                if (findItemDescription == null)
                    item.Descriptions.Add(itemDescription);
                else
                    findItemDescription.Description = itemDescription.Description;
            }
        }

        void AssignBrandIdsFromBrandNames(Item item, NewItemViewModel newItemViewModel, List<Brand> allBrands, bool deleteOld = true)
        {
            if (deleteOld)
                item.BrandsItems.Clear();

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

                if (item.BrandsItems.All(i => i.Brand.Name != brandName))
                {
                    var newBrandItem = new BrandItem()
                    {
                        Item = item,
                        Brand = findBrand
                    };

                    item.BrandsItems.Add(newBrandItem);
                }
            }

            foreach (var brandId in newItemViewModel.BrandIds)
            {
                if (item.BrandsItems.All(i => i.BrandId != brandId))
                {
                    var newBrandItem = new BrandItem()
                    {
                        Item = item,
                        BrandId = brandId
                    };
                    item.BrandsItems.Add(newBrandItem);
                }
            }

        }
    }
}
