using Microsoft.EntityFrameworkCore;
using SpletnaTrgovinaDiploma.Data.Base;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public class CountryService : EntityBaseRepository<Country>, ICountryService
    {
        private readonly AppDbContext context;

        public CountryService(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<CountryDropdownViewModel> GetDropdownValuesAsync()
        {
            var response = new CountryDropdownViewModel
            {
                Countries = await context.Countries
                    .OrderBy(n => n.Name)
                    .ToListAsync()
            };

            return response;
        }
    }
}
