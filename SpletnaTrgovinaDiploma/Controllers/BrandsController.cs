using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Data.Static;
using SpletnaTrgovinaDiploma.Models;
using System.Linq;
using System.Threading.Tasks;
using SpletnaTrgovinaDiploma.Helpers;
using X.PagedList;

namespace SpletnaTrgovinaDiploma.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class BrandsController : Controller
    {
        private readonly IBrandsService brandsService;

        public BrandsController(IBrandsService brandsService)
        {
            this.brandsService = brandsService;
        }

        public IActionResult EditIndex(string currentFilter, string searchString, int page = 1)
        {
            var filteredBrands = GetFilteredBrands(currentFilter, searchString, page);
            return View(filteredBrands);
        }

        IPagedList<Brand> GetFilteredBrands(string currentFilter, string searchString, int page)
        {
            const int itemsPerPage = 12;

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;
            var allBrands = brandsService
                .GetAll(n => n.BrandsItems);

            if (!string.IsNullOrEmpty(searchString))
            {
                var upperCaseSearchString = searchString.ToUpper();
                var filteredResult = allBrands
                    .Where(n => n.Name.ToUpper().Contains(upperCaseSearchString));

                ViewData.SetPageDetails("Search result", $"Search result for \"{searchString}\"");
                return filteredResult.ToPagedList(page, itemsPerPage);
            }

            ViewData.SetPageDetails("Home page", "Home page of Gaming svet");
            return allBrands.ToPagedList(page, itemsPerPage);
        }

        public IActionResult Create()
            => View();

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,ProfilePictureUrl")] Brand brand)
        {
            if (!ModelState.IsValid)
                return View(brand);

            await brandsService.AddAsync(brand);
            return RedirectToAction(nameof(EditIndex));
        }

        public async Task<IActionResult> Details(int id)
        {
            var brandDetails = await brandsService.GetByIdAsync(id);

            if (brandDetails == null)
                return View("NotFound");

            return View(brandDetails);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var brandDetails = await brandsService.GetByIdAsync(id);

            if (brandDetails == null)
                return View("NotFound");

            return View(brandDetails);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ProfilePictureUrl")] Brand brand)
        {
            if (!ModelState.IsValid)
                return View(brand);

            await brandsService.UpdateAsync(id, brand);
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var brandDetails = await brandsService.GetByIdAsync(id);

            if (brandDetails == null)
                return View("NotFound");

            return View(brandDetails);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brandDetails = await brandsService.GetByIdAsync(id);

            if (brandDetails == null)
                return View("NotFound");

            await brandsService.DeleteAsync(id);
            return RedirectToAction(nameof(EditIndex));
        }
    }
}
