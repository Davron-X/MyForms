using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyForms.Data;
using MyForms.Models;
using MyForms.Services;
using MyForms.Services.Interfaces;

namespace MyForms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<ITemplateService, TemplateService>();
            builder.Services.AddControllersWithViews();
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
            app.Run();
        }
    }
}
