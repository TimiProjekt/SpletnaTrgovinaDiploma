﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpletnaTrgovinaDiploma.Data.Base;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public class NewsletterEmailService : EntityBaseRepository<NewsletterEmail>, INewsletterEmailService
    {
        private readonly AppDbContext context;

        public NewsletterEmailService(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task AddToMailingList(string emailAddress)
        {
            // If it already exists, don't add it again. Future: Give feedback that it's already registered
            if (context.NewsletterMailingList.Any(newsletterEmail => newsletterEmail.Email == emailAddress))
                return;

            var newItem = new NewsletterEmail()
            {
                Email = emailAddress
            };

            await context.NewsletterMailingList.AddAsync(newItem);
            await context.SaveChangesAsync();
        }

        public async Task RemoveFromMailingList(string emailAddress)
        {
            // If it doesn't exist, don't remove it. Future: Give feedback that it's not subscribed
            var newsletterEmail = context.NewsletterMailingList.Single(e => e.Email == emailAddress);
            if (newsletterEmail == null)
                return;

            context.NewsletterMailingList.Remove(newsletterEmail);
            await context.SaveChangesAsync();
        }

        public IEnumerable<NewsletterEmail> GetAllEmailAddressesFromMailingList()
        {
            return context.NewsletterMailingList.Select(c => c);
        }
    }
}
