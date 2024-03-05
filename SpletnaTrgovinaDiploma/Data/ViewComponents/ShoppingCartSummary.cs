using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpletnaTrgovinaDiploma.Data.Services.Classes;

namespace SpletnaTrgovinaDiploma.Data.ViewComponents
{
    public class ShoppingCartSummary : ViewComponent
    {
        private readonly ShoppingCartService shoppingCartService;

        public ShoppingCartSummary(ShoppingCartService shoppingCartService)
        {
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var shoppingCartViewModel = await shoppingCartService.GetShoppingCartViewModel();

            return View(shoppingCartViewModel.TotalAmountOfItems);
        }
    }
}
