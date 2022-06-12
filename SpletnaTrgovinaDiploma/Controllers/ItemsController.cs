using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpletnaTrgovinaDiploma.Data;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly IItemsService _service;

        public ItemsController(IItemsService service) 
        {
            _service = service;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index() 
        {
            var allItems = await _service.GetAllAsync(n => n.Brands_Items);
            return View(allItems);
        }

        public async Task<IActionResult> Filter(string searchString)
        {
            var allItems = await _service.GetAllAsync(n => n.Brands_Items);

            if (!string.IsNullOrEmpty(searchString))
            {
                var upperCaseSearchString = searchString.ToUpper();
                var filteredResult = allItems.Where(n => n.Name.ToUpper().Contains(upperCaseSearchString) || n.Description.ToUpper().Contains(upperCaseSearchString));
                return View("Index", filteredResult);
            }

            return View("Index", allItems);
        }

        //GET: Items/Details/1(ItemId)
        [AllowAnonymous]
        public async Task <IActionResult> Details(int id) 
        {
            var itemDetail = await _service.GetItemByIdAsync(id);
            return View(itemDetail);
        }

        //GET: Movies/Create
        public async Task<IActionResult> Create() 
        {
            var itemDropdownsData = await _service.GetNewItemDropdownsValues();

            ViewBag.Brands = new SelectList(itemDropdownsData.Brands, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewItemVM item)
        {
            if (!ModelState.IsValid)
            {
                var itemDropdownsData = await _service.GetNewItemDropdownsValues();

                ViewBag.Brands = new SelectList(itemDropdownsData.Brands, "Id", "Name");

                return View(item);
            }

            await _service.AddNewItemAsync(item);
            return RedirectToAction(nameof(Index));
        }

        //Get: Items/Edit/1
        public async Task<IActionResult> Edit(int id)
        {
            var itemDetails = await _service.GetItemByIdAsync(id);
            if (itemDetails == null) return View("Not Found");

            var response = new NewItemVM()
            {
                Id = itemDetails.Id,
                Name = itemDetails.Name,
                Description = itemDetails.Description,
                Price = itemDetails.Price,
                ImageURL = itemDetails.ImageURL,
                ItemCategory = itemDetails.ItemCategory,
                BrandIds = itemDetails.Brands_Items.Select(n => n.BrandId).ToList(),
            };


            var itemDropdownsData = await _service.GetNewItemDropdownsValues();

            ViewBag.Brands = new SelectList(itemDropdownsData.Brands, "Id", "Name");

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, NewItemVM item)
        {
            if (id != item.Id) return View("NotFound");

            if (!ModelState.IsValid)
            {
                var itemDropdownsData = await _service.GetNewItemDropdownsValues();

                ViewBag.Brands = new SelectList(itemDropdownsData.Brands, "Id", "Name");

                return View(item);
            }

            await _service.UpdateItemAsync(item);
            return RedirectToAction(nameof(Index));
        }

    }
}
