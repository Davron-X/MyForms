using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyForms.Models;
using MyForms.Models.DTOs;

namespace MyForms.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        private readonly SignInManager<ApplicationUser> signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationDto registrationDto)
        {
            if(!ModelState.IsValid)
                return View();

            ApplicationUser applicationUser = new()
            {
                UserName=registrationDto.Email,
                Email = registrationDto.Email,
                FullName = registrationDto.FullName
            };
            IdentityResult identityResult= await userManager.CreateAsync(applicationUser,registrationDto.Password);
            if (identityResult.Succeeded)
            {
                await signInManager.SignInAsync(applicationUser, registrationDto.IsRemeber);                
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto,string? returnUrl=null)
        {
            if (!ModelState.IsValid)
                return View();

            var user= await userManager.FindByEmailAsync(loginDto.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user,loginDto.Password))
            {
                ModelState.AddModelError("", "Email or password is incorrect");
                return View();
            }
            await signInManager.SignInAsync(user,  loginDto.IsRemeber);
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index","Home");

            return LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
    }
}
