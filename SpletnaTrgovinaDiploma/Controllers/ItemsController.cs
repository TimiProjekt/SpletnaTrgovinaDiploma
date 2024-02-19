using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index(string currentFilter, string searchString, int page = 1)
        {
            ViewData.SetPageDetails("Home page", "");

            var filteredItems = GetFilteredItems(currentFilter, searchString, page);
            ViewBag.CurrentItemsFilter = searchString;
            return View(filteredItems);
        }

        public IActionResult EditIndex(string currentFilter, string searchString, int page = 1)
        {
            ViewData.SetPageDetails("Items page", "Items overview");

            var filteredItems = GetFilteredItems(currentFilter, searchString, page);
            return View(filteredItems);
        }

        IPagedList<Item> GetFilteredItems(string currentFilter, string searchString, int page)
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
                var filteredResult = allItems
                    .AsEnumerable()
                    .Where(i => i.Name.ContainsCaseInsensitive(searchString)
                                || i.Description.ContainsCaseInsensitive(searchString)
                                || i.ShortDescription.ContainsCaseInsensitive(searchString)
                                || i.ProductCode.ContainsCaseInsensitive(searchString));

                ViewData.SetPageDetails("Search result", $"Search result for \"{searchString}\"");
                return filteredResult.ToPagedList(page, itemsPerPage);
            }

            return allItems.ToPagedList(page, itemsPerPage);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var itemDetail = await itemsService.GetItemByIdAsync(id);

            if (itemDetail == null)
                return View("NotFound");

            return View(itemDetail);
        }

        public async Task<IActionResult> Create()
        {
            await DropdownUtil.LoadBrandsDropdownData(brandService, ViewBag);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewItemViewModel item)
        {
            if (!ModelState.IsValid)
            {
                await DropdownUtil.LoadBrandsDropdownData(brandService, ViewBag);

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

            await DropdownUtil.LoadBrandsDropdownData(brandService, ViewBag);
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, NewItemViewModel item)
        {
            if (id != item.Id)
                return View("NotFound");

            if (!ModelState.IsValid)
            {
                await DropdownUtil.LoadBrandsDropdownData(brandService, ViewBag);

                return View(item);
            }

            await itemsService.UpdateItemAsync(item);
            return RedirectToAction(nameof(Details), new { id });
        }

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

        public IActionResult Import()
            => View(new ImportXmlModel());

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

            return View("Success", new SuccessViewModel($"Successfully {(importXmlModel.IsUpdateExisting ? "updated" : "imported")} {amountOfItems} items.", ""));
        }
    }
}
