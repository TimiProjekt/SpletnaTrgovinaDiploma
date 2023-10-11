using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class ItemsController : Controller
    {
        private readonly IItemsService service;
        private readonly IBrandsService brandService;

        public ItemsController(IItemsService service, IBrandsService brandService)
        {
            this.service = service;
            this.brandService = brandService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetByBrand(int id)
        {
            var allItems = await service.GetAllAsync(n => n.BrandsItems);
            var filteredItems = allItems.Where(item => item.BrandsItems.Any(bi => bi.BrandId == id));
            return View(filteredItems);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var allItems = await service.GetAllAsync(n => n.BrandsItems);
            SetPageDetails("Home page", "Home page of Fire Computer Parts");
            return View(allItems);
        }

        public async Task<IActionResult> EditIndex()
        {
            var data = await service.GetAllAsync();
            var orderedData = data.OrderBy(item => item.Name);
            return View(orderedData);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Filter(string searchString)
        {
            var allItems = await service.GetAllAsync(n => n.BrandsItems);

            if (!string.IsNullOrEmpty(searchString))
            {
                var upperCaseSearchString = searchString.ToUpper();
                var filteredResult = allItems
                    .Where(n => n.Name.ToUpper().Contains(upperCaseSearchString) || n.Description.ToUpper().Contains(upperCaseSearchString));

                SetPageDetails("Search result", $"Search result for \"{searchString}\"");
                return View("Index", filteredResult);
            }

            SetPageDetails("Home page", "Home page of Fire Computer Parts");
            return View("Index", allItems);
        }

        //GET: Items/Details/1(ItemId)
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var itemDetail = await service.GetItemByIdAsync(id);
            return View(itemDetail);
        }

        //GET: Movies/Create
        public async Task<IActionResult> Create()
        {
            var itemDropdownsData = await brandService.GetDropdownValuesAsync();

            ViewBag.Brands = new SelectList(itemDropdownsData.Brands, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewItemViewModel item)
        {
            if (!ModelState.IsValid)
            {
                var itemDropdownsData = await brandService.GetDropdownValuesAsync();

                ViewBag.Brands = new SelectList(itemDropdownsData.Brands, "Id", "Name");

                return View(item);
            }

            await service.AddNewItemAsync(item);
            return RedirectToAction(nameof(Index));
        }

        //Get: Items/Edit/1
        public async Task<IActionResult> Edit(int id)
        {
            var itemDetails = await service.GetItemByIdAsync(id);
            if (itemDetails == null)
                return View("NotFound");

            var response = new NewItemViewModel()
            {
                Id = itemDetails.Id,
                Name = itemDetails.Name,
                Description = itemDetails.Description,
                ShortDescription = itemDetails.ShortDescription,
                Price = itemDetails.Price,
                ImageUrl = itemDetails.ImageUrl,
                ItemCategory = itemDetails.ItemCategory,
                BrandIds = itemDetails.BrandsItems.Select(n => n.BrandId).ToList(),
            };


            var itemDropdownsData = await brandService.GetDropdownValuesAsync();

            ViewBag.Brands = new SelectList(itemDropdownsData.Brands, "Id", "Name");

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, NewItemViewModel item)
        {
            if (id != item.Id) 
                return View("NotFound");

            if (!ModelState.IsValid)
            {
                var itemDropdownsData = await brandService.GetDropdownValuesAsync();

                ViewBag.Brands = new SelectList(itemDropdownsData.Brands, "Id", "Name");

                return View(item);
            }

            await service.UpdateItemAsync(item);
            return RedirectToAction(nameof(Index));
        }

        void SetPageDetails(string title, string description)
        {
            ViewData["Title"] = title;
            ViewData["Description"] = description;
        }

        //Get: Items/Delete/1
        public async Task<IActionResult> Delete(int id)
        {
            var itemDetails = await service.GetByIdAsync(id);

            if (itemDetails == null)
                return View("NotFound");

            return View(itemDetails);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemDetails = await service.GetByIdAsync(id);

            if (itemDetails == null)
                return View("NotFound");

            await service.DeleteAsync(id);
            return RedirectToAction(nameof(EditIndex));
        }
    }
}
