﻿using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Helpers;
using X.PagedList;

namespace SpletnaTrgovinaDiploma.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class ItemsController : Controller
    {
        private readonly IItemsService itemsService;
        private readonly IBrandsService brandService;
        private readonly XmlImportUtil xmlImportUtil;

        public ItemsController(IItemsService itemsService, IBrandsService brandService)
        {
            this.itemsService = itemsService;
            this.brandService = brandService;

            xmlImportUtil = new XmlImportUtil(itemsService);
        }

        [AllowAnonymous]
        public IActionResult GetByBrand(int id)
        {
            var allItems = itemsService.GetAll(n => n.BrandsItems);
            var filteredItems = allItems.Where(item => item.BrandsItems.Any(bi => bi.BrandId == id));
            return View(filteredItems);
        }

        public IActionResult EditIndex()
        {
            var data = itemsService.GetAll();
            var orderedData = data.OrderBy(item => item.Name);
            return View(orderedData);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string currentFilter, string searchString, int page = 1)
        {
            const int itemsPerPage = 12;

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;
            var allItems = itemsService
                .GetAll(n => n.BrandsItems);

            if (!string.IsNullOrEmpty(searchString))
            {
                var filteredItems = allItems
                    .Where(i => ContainsSearchString(i, searchString));

                SetPageDetails("Search result", $"Search result for \"{searchString}\"");
                return View("Index", filteredItems.ToPagedList(page, itemsPerPage));
            }

            SetPageDetails("Home page", "Home page of Gaming svet");
            return View("Index", allItems.ToPagedList(page, itemsPerPage));
        }

        static bool ContainsSearchString(Item item, string searchString)
            => !string.IsNullOrEmpty(item.Name) && item.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)
                || !string.IsNullOrEmpty(item.Description) && item.Description.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)
                || !string.IsNullOrEmpty(item.ShortDescription) && item.ShortDescription.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)
                || !string.IsNullOrEmpty(item.ProductCode) && item.ProductCode.Contains(searchString, StringComparison.InvariantCultureIgnoreCase);

        public IActionResult IndexAdmin(string searchString)
        {
            var allItems = itemsService.GetAll(n => n.BrandsItems);

            if (!string.IsNullOrEmpty(searchString))
            {
                var upperCaseSearchString = searchString.ToUpper();
                var filteredResult = allItems
                    .Where(i => ContainsSearchString(i, searchString));

                SetPageDetails("Search result", $"Search result for \"{searchString}\"");
                return View("EditIndex", filteredResult);
            }

            SetPageDetails("Items", "Search result");
            return View("EditIndex", allItems);
        }

        //GET: Items/Details/1(ItemId)
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var itemDetail = await itemsService.GetItemByIdAsync(id);

            if (itemDetail == null)
                return View("NotFound");

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

            await itemsService.AddNewItemAsync(item);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var itemDetails = await itemsService.GetItemByIdAsync(id);
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
                ProductCode = itemDetails.ProductCode,
                Availability = itemDetails.Availability
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

            await itemsService.UpdateItemAsync(item);
            return RedirectToAction(nameof(Details), new { id });
        }

        void SetPageDetails(string title, string description)
        {
            ViewData["Title"] = title;
            ViewData["Description"] = description;
        }

        //Get: Items/Delete/1
        public async Task<IActionResult> Delete(int id)
        {
            var itemDetails = await itemsService.GetByIdAsync(id);

            if (itemDetails == null)
                return View("NotFound");

            return View(itemDetails);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemDetails = await itemsService.GetByIdAsync(id);

            if (itemDetails == null)
                return View("NotFound");

            await itemsService.DeleteAsync(id);
            return RedirectToAction(nameof(EditIndex));
        }

        public IActionResult Import() => View(new ImportXmlModel());

        [HttpPost]
        public async Task<IActionResult> Import(ImportXmlModel importXmlModel)
        {
            if (!ModelState.IsValid)
                return View(importXmlModel);

            var doc = new XmlDocument();

            doc.Load(importXmlModel.File.OpenReadStream());
            if (doc.DocumentElement == null)
            {
                ModelState.AddModelError(nameof(importXmlModel.File), "XML is not in the expected format.");
                return View(importXmlModel);
            }

            var amountOfItems = await xmlImportUtil.TryImportItemDetails(doc, importXmlModel.IsUpdateExisting);
            amountOfItems += await xmlImportUtil.TryImportAvailability(doc, importXmlModel.IsUpdateExisting);

            return View("Success", new EmailViewModel($"Successfully {(importXmlModel.IsUpdateExisting ? "updated" : "imported")} {amountOfItems} items.", ""));
        }
    }
}
