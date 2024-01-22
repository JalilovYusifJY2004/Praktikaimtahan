using Microsoft.AspNetCore.Identity;

namespace Praktikabitdi.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string SurName { get; set; }
    }
}
