using System.Collections.Generic;
using System.Threading.Tasks;
using SpletnaTrgovinaDiploma.Data.Base;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public interface INewsletterEmailService : IEntityBaseRepository<NewsletterEmail>
    {
        IEnumerable<NewsletterEmail> AllEmailAddressesFromMailingList { get; }

        Task<bool> AddToMailingListAsync(string emailAddress);

        Task<bool> RemoveFromMailingListAsync(string emailAddress);

    }
}
