using System.Collections.Generic;
using System.Threading.Tasks;
using SpletnaTrgovinaDiploma.Data.Base;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public interface INewsletterEmailService : IEntityBaseRepository<NewsletterEmail>
    {
        Task AddToMailingList(string emailAddress);

        IEnumerable<NewsletterEmail> GetAllEmailAddressesFromMailingList();
    }
}
