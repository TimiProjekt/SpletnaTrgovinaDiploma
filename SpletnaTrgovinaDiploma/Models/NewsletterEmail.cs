using System.ComponentModel.DataAnnotations;
using SpletnaTrgovinaDiploma.Data.Base;

namespace SpletnaTrgovinaDiploma.Models
{
    public class NewsletterEmail : IEntityBase
    {
        public int Id { get; set; }

        [Display(Name = "Email address")]
        public string Email { get; set; }
    }
}
