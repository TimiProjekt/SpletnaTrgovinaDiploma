using SpletnaTrgovinaDiploma.Data.Base;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public interface ICountryService : IEntityBaseRepository<Country>
    {
        Task<CountryDropdownViewModel> GetDropdownValuesAsync();
    }
}
