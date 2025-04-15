using Imagekit.Sdk;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyForms.Data;
using MyForms.Filters;
using MyForms.Models;
using MyForms.Services;
using MyForms.Services.Interfaces;
using System.Threading.Tasks;

namespace MyForms
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<ITemplateService, TemplateService>();
            builder.Services.AddSingleton<ImagekitClient>(new ImagekitClient(
                publicKey: "public_2Iw5d3wY3LuUOliT7zMl1cdjNPE=",
                privateKey: "private_0msYRD6HMOspGVhL0WBZYwWHFz0=",
                urlEndPoint: "https://ik.imagekit.io/k9moe0d70" 
             ));
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new TypeFilterAttribute(typeof(UserStatusCheckFilter)));
            });
            builder.Services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
            });
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.Password.RequiredLength = 1;
                option.Password.RequireUppercase=false;
                option.Password.RequireLowercase=false;
                option.Password.RequireDigit=false;
                option.Password.RequireNonAlphanumeric=false;

            }).AddEntityFrameworkStores<ApplicationDbContext>();
            var app = builder.Build();

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapDefaultControllerRoute();
            using(var scope = app.Services.CreateScope())
            {
                var services= scope.ServiceProvider;
                var roleManager= services. GetRequiredService<RoleManager<IdentityRole>>();
                var userManager= services.GetRequiredService<UserManager<ApplicationUser>>();
                await IdentityInitializer.Initialize(roleManager, userManager);
            }
            app.Run();
        }
    }
}
