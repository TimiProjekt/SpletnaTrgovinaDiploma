﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Models;
using System.Linq;
using System.Threading.Tasks;
using SpletnaTrgovinaDiploma.Helpers;

namespace SpletnaTrgovinaDiploma.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class BrandsController : Controller
    {
        private readonly IBrandsService service;

        public BrandsController(IBrandsService service)
        {
            this.service = service;
        }

        public IActionResult EditIndex()
        {
            var data = service.GetAll();
            var orderedData = data.OrderBy(brand => brand.Name);

            return View(orderedData);
        }

        public IActionResult Filter(string searchString)
        {
            var allBrands = service.GetAll(n => n.BrandsItems);

            if (!string.IsNullOrEmpty(searchString))
            {
                var upperCaseSearchString = searchString.ToUpper();
                var filteredResult = allBrands
                    .Where(n => n.Name.ToUpper().Contains(upperCaseSearchString));

                ViewData.SetPageDetails("Search result", $"Search result for \"{searchString}\"");
                return View("EditIndex", filteredResult);
            }

            ViewData.SetPageDetails("Home page", "Home page of Gaming svet");
            return View("EditIndex", allBrands);
        }

        public IActionResult Create()
            => View();

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,ProfilePictureUrl")] Brand brand)
        {
            if (!ModelState.IsValid)
                return View(brand);

            await service.AddAsync(brand);
            return RedirectToAction(nameof(EditIndex));
        }

        public async Task<IActionResult> Details(int id)
        {
            var brandDetails = await service.GetByIdAsync(id);

            if (brandDetails == null)
                return View("NotFound");

            return View(brandDetails);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var brandDetails = await service.GetByIdAsync(id);

            if (brandDetails == null)
                return View("NotFound");

            return View(brandDetails);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ProfilePictureUrl")] Brand brand)
        {
            if (!ModelState.IsValid)
                return View(brand);

            await service.UpdateAsync(id, brand);
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var brandDetails = await service.GetByIdAsync(id);

            if (brandDetails == null)
                return View("NotFound");

            return View(brandDetails);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brandDetails = await service.GetByIdAsync(id);

            if (brandDetails == null)
                return View("NotFound");

            await service.DeleteAsync(id);
            return RedirectToAction(nameof(EditIndex));
        }
    }
}
