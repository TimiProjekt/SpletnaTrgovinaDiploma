using Microsoft.EntityFrameworkCore;
using SpletnaTrgovinaDiploma.Data.Base;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public class ItemsService:EntityBaseRepository<Item>, IItemsService
    {
        private readonly AppDbContext _context;
        public ItemsService(AppDbContext context) : base(context) 
        {
            _context = context;
        }

        public async Task AddNewItemAsync(NewItemVM data)
        {
            var newItem = new Item()
            {
                Name = data.Name,
                Description = data.Description,
                Price = data.Price,
                ImageURL = data.ImageURL,
                ItemCategory = data.ItemCategory
            };
            await _context.Items.AddAsync(newItem);
            await _context.SaveChangesAsync();

            //Add Item Brands
            foreach (var brandId in data.BrandIds)
            {
                var newBrandItem = new Brand_Item()
                {
                    ItemId = newItem.Id,
                    BrandId = brandId
                };
                await _context.Brands_Items.AddAsync(newBrandItem);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            var itemDetails = await _context.Items
                .Include(am => am.Brands_Items).ThenInclude(b => b.Brand)
                .FirstOrDefaultAsync(n => n.Id == id);

            return itemDetails;
        }

        public async Task<NewItemDropdownsVM> GetNewItemDropdownsValues()
        {
            var response = new NewItemDropdownsVM()
            {
                Brands = await _context.Brands.OrderBy(n => n.Name).ToListAsync()
            };

            return response;
        }

        public async Task UpdateItemAsync(NewItemVM data)
        {
            var dbItem = await _context.Items.FirstOrDefaultAsync(n => n.Id == data.Id);

            if(dbItem != null)
            {
                dbItem.Name = data.Name;
                dbItem.Description = data.Description;
                dbItem.Price = data.Price;
                dbItem.ImageURL = data.ImageURL;
                dbItem.ItemCategory = data.ItemCategory;
                await _context.SaveChangesAsync();
            }

            //Remove existing items
            var existingBrandsDb = _context.Brands_Items.Where(n => n.ItemId == data.Id).ToList();
             _context.Brands_Items.RemoveRange(existingBrandsDb);
            await _context.SaveChangesAsync();

            //Add Item Brands
            foreach (var brandId in data.BrandIds)
            {
                var newBrandItem = new Brand_Item()
                {
                    ItemId = data.Id,
                    BrandId = brandId
                };
                await _context.Brands_Items.AddAsync(newBrandItem);
            }
            await _context.SaveChangesAsync();
        }
    }
}
