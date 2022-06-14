using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
    public class CategoryController : Controller
    {
        private readonly IItemsService _service;

        public CategoryController(IItemsService service) 
        {
            _service = service;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int id) 
        {
            var allItems = await _service.GetAllAsync(n => n.Brands_Items);
            var categoryItems = allItems.Where(item => (int)item.ItemCategory == id);
            return View(categoryItems);
        }
    }
}
