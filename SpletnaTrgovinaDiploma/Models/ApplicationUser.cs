using Microsoft.AspNetCore.Identity;

namespace SpletnaTrgovinaDiploma.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DeliveryInfo DeliveryInfo { get; set; }
    }
}
