using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Models;
using System.Linq;
using System.Threading.Tasks;

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

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data = await service.GetAllAsync();
            var orderedData = data.OrderBy(brand => brand.Name);
            return View(orderedData);
        }

        //Get: Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,ProfilePictureUrl")] Brand brand)
        {
            if (!ModelState.IsValid)
                return View(brand);
            
            await service.AddAsync(brand);
            return RedirectToAction(nameof(Index));
        }

        //Get: Brands/Details/1
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var brandDetails = await service.GetByIdAsync(id);

            if (brandDetails == null) 
                return View("NotFound");

            return View(brandDetails);
        }

        //Get: Brands/Edit/1
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
            return RedirectToAction(nameof(Index));
        }

        //Get: Brands/Delete/1
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
            return RedirectToAction(nameof(Index));
        }
    }
}
