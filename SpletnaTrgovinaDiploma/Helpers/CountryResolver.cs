using System.Linq;
using SpletnaTrgovinaDiploma.Data;

namespace SpletnaTrgovinaDiploma.Helpers
{
    public class CountryResolver
    {

        public static string GetCountryNameFromId(AppDbContext context, int countryId)
        {
            return context.Countries.SingleOrDefault(c => c.Id == countryId)?.Name;
        }
    }
}
