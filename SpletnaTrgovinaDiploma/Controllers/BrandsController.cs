using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpletnaTrgovinaDiploma.Data;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class BrandsController : Controller
    {
        private readonly IBrandsService _service;

        public BrandsController(IBrandsService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            var orderData = data.OrderBy(brand => brand.Name);
            return View(orderData);
        }

        //Get: Brands/Create
        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,ProfilePictureURL")] Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View(brand);
            }
            await _service.AddAsync(brand);
            return RedirectToAction(nameof(Index));
        }

        //Get: Brands/Details/1
        [AllowAnonymous]
        public async Task <IActionResult> Details (int id) 
        {
            var brandDetails = await _service.GetByIdAsync(id);

            if (brandDetails == null) return View("NotFound");
            return View(brandDetails);
        }

        //Get: Brands/Edit/1
        public async Task<IActionResult> Edit(int id)
        {
            var brandDetails = await _service.GetByIdAsync(id);
            if (brandDetails == null) return View("NotFound");
            return View(brandDetails);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,[Bind("Id,Name,ProfilePictureURL")] Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View(brand);
            }
            await _service.UpdateAsync(id,brand);
            return RedirectToAction(nameof(Index));
        }

        //Get: Brands/Delete/1
        public async Task<IActionResult> Delete(int id)
        {
            var brandDetails = await _service.GetByIdAsync(id);
            if (brandDetails == null) return View("NotFound");
            return View(brandDetails);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brandDetails = await _service.GetByIdAsync(id);
            if (brandDetails == null) return View("NotFound");

            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
