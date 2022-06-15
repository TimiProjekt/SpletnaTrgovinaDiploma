using SpletnaTrgovinaDiploma.Data.Base;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public class BrandsService : EntityBaseRepository<Brand>, IBrandsService
    {
        public BrandsService(AppDbContext context) : base(context) { }
    }
}
