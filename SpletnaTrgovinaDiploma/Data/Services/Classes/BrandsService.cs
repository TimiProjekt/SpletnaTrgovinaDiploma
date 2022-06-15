using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpletnaTrgovinaDiploma.Data.Base;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public class BrandsService : EntityBaseRepository<Brand>, IBrandsService
    {
        private readonly AppDbContext context;

        public BrandsService(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<BrandDropdownViewModel> GetDropdownValuesAsync()
        {
            var response = new BrandDropdownViewModel()
            {
                Brands = await context.Brands.OrderBy(n => n.Name).ToListAsync()
            };

            return response;
        }
    }
}
