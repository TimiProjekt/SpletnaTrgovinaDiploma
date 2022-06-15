using System.Threading.Tasks;
using SpletnaTrgovinaDiploma.Data.Base;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public interface IBrandsService : IEntityBaseRepository<Brand>
    {
        Task<BrandDropdownViewModel> GetDropdownValuesAsync();
    }
}
