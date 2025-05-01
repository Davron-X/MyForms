using Humanizer.Localisation;
using Imagekit.Sdk;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
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
            
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddControllersWithViews()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResource));
            });

            builder.Services.AddSingleton<ImagekitClient>(new ImagekitClient(
                publicKey: "public_2Iw5d3wY3LuUOliT7zMl1cdjNPE=",
                privateKey: "private_0msYRD6HMOspGVhL0WBZYwWHFz0=",
                urlEndPoint: "https://ik.imagekit.io/k9moe0d70" 
             ));
         

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
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.AccessDeniedPath = "/Auth/AccessDenied";
            });
            
            builder.Services.AddScoped<ITemplateService, TemplateService>();
            builder.Services.AddScoped<IFormService, FormService>();
            builder.Services.AddScoped<IHomeService, HomeService>();
            builder.Services.AddScoped<IUserService, UserService>();
            var app = builder.Build();
            app.UseDeveloperExceptionPage();
            var supportedCultures = new[] { "en", "ru" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture("en")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseStaticFiles();
            app.UseRequestLocalization(localizationOptions);

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapDefaultControllerRoute();

           
            using (var scope = app.Services.CreateScope())
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
