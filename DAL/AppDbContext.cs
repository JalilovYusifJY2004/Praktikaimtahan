using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Praktikabitdi.Models;

namespace Praktikabitdi.DAL
{
    public class AppDbcontext:IdentityDbContext<AppUser>
    {
        public AppDbcontext(DbContextOptions<AppDbcontext> options):base(options) { }
        public DbSet<Team> Teams { get; set; }
    }
}
