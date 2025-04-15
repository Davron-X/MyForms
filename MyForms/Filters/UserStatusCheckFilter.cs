using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using MyForms.Models;
using System.Security.Claims;

namespace MyForms.Filters
{
    public class UserStatusCheckFilter : IAsyncAuthorizationFilter
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserStatusCheckFilter(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (!user.Identity.IsAuthenticated || actionDescriptor?.ControllerTypeInfo.Name=="AuthController")
            {
                return;
            }
            string? userId= user.FindFirstValue(ClaimTypes.NameIdentifier);
            if ( userId is null)
            {
                await signInManager.SignOutAsync();
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }
            ApplicationUser? userFromDb= await userManager.FindByIdAsync(userId);
            if (userFromDb is null || userFromDb.LockoutEnd > DateTime.UtcNow)
            {
                await signInManager.SignOutAsync();
                context.Result = new RedirectToActionResult("Index","Home",null);
                return;
            }

        }
    }
}
