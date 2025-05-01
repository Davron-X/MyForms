using Microsoft.AspNetCore.Identity;
using MyForms.Models;

namespace MyForms
{
    public static class IdentityInitializer
    {
        public static async Task Initialize(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
        {
            if(await roleManager.RoleExistsAsync(AppConsts.AdminRole)){
                return;
            }
            await roleManager.CreateAsync(new IdentityRole(AppConsts.AdminRole));
            await roleManager.CreateAsync(new IdentityRole(AppConsts.UserRole));

            if (await userManager.FindByEmailAsync(AppConsts.AdminEmail) == null)
            {
                ApplicationUser admin = new()
                {
                    FullName="AdminAka",
                    Email = AppConsts.AdminEmail,
                    UserName = AppConsts.AdminEmail,
                    EmailConfirmed = true
                };
                var result=await userManager.CreateAsync(admin, AppConsts.AdminPassword);
                if (result.Succeeded)
                {
                   await userManager.AddToRoleAsync(admin, AppConsts.AdminRole);
                }
            }
        }
    }
}
