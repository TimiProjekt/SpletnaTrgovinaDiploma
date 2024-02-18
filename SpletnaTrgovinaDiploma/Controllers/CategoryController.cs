using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpletnaTrgovinaDiploma.Data.Services;
using System.Linq;
using SpletnaTrgovinaDiploma.Data;

namespace SpletnaTrgovinaDiploma.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IItemsService service;

        public CategoryController(IItemsService service)
        {
            this.service = service;
        }

        [AllowAnonymous]
        public IActionResult Index(int id)
        {
            var allItems = service.GetAll(n => n.BrandsItems);
            var categoryItems = allItems.Where(item => item.ItemCategory == (ItemCategory)id);

            return View(categoryItems);
        }
    }
}
