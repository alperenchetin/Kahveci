using Microsoft.AspNetCore.Identity;

namespace Kahveci_API.Entities
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
