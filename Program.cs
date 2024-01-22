using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Praktikabitdi.DAL;
using Praktikabitdi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbcontext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric=false;
    options.Password.RequiredLength= 8;
    options.User.RequireUniqueEmail= true;
    options.Lockout.MaxFailedAccessAttempts= 5;
}).AddEntityFrameworkStores<AppDbcontext>().AddDefaultTokenProviders();

var app = builder.Build();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllerRoute(
    "Default",
    "{area:exists}/{controller=home}/{action=index}/{id?}");
app.MapControllerRoute(
    "Default",
    "{controller=home}/{action=index}/{id?}");

app.Run();
