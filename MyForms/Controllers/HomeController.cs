using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyForms.Data;
using MyForms.Models;
using MyForms.Models.VM;
using MyForms.Models.VM.HomeVMs;
using MyForms.Services.Interfaces;
using System.Threading.Tasks;

namespace MyForms.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService homeService;

        public HomeController(ApplicationDbContext db, IHomeService homeService)
        {
            this.homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var homeVM = await homeService.GetHomeViewModelAsync();
            return View(homeVM);
        }
        public IActionResult SetCulture(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }


        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return View(new List<TemplateSearchResult>());

            ViewBag.Query = query;
            var results = homeService.SearchTemplates(query);
            return View(results);
        }

    }

}
