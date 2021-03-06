using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpletnaTrgovinaDiploma.Data.Services;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index(int id)
        {
            var allItems = await service.GetAllAsync(n => n.BrandsItems);
            var categoryItems = allItems.Where(item => (int)item.ItemCategory == id);
            return View(categoryItems);
        }
    }
}
