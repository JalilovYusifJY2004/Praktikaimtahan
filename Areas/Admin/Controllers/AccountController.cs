using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Praktikabitdi.Areas.Admin.ViewModels.Account;
using Praktikabitdi.Models;
using Praktikabitdi.Utilities.Enum;

namespace Praktikabitdi.Areas.Admin.Controllers;


[Area("Admin")]
public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signIn;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signIn, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signIn = signIn;
        _roleManager = roleManager;
    }
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM registerVM)
    {
        if (!ModelState.IsValid)
        {
            return View(registerVM);
        }
        AppUser user = new AppUser
        {
            Name = registerVM.Name,
            Email = registerVM.Email,
            SurName = registerVM.SurName,
            UserName = registerVM.UserName,
        };
        IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);
        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                return View();
            }

        }
        await _signIn.SignInAsync(user, false);
        return RedirectToAction("Index", "Home", new { Area = "" });
    }
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM loginVM)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        AppUser user = await _userManager.FindByEmailAsync(loginVM.UserNameorEmail);
        if (user == null)
        {
            user = await _userManager.FindByNameAsync(loginVM.UserNameorEmail);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "tapilmadi");
                return View();
            }
        }


        var result = await _signIn.PasswordSignInAsync(user, loginVM.Password, loginVM.IsRemembered, true);
        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "bloklanib");
            return View();
        }
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "username or email incorrect");
            return View();

        }

        return RedirectToAction("Index", "Home", new { Area = "" });



    }
    public async Task<IActionResult> CreateRoles()
    {
        foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
        {
            if (!(await _roleManager.RoleExistsAsync(role.ToString())))
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = role.ToString(),
                });
            }
        }
        return RedirectToAction("Index", "Home", new { Area = "" });
    }

}