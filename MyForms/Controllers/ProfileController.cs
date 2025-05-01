using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyForms.Data;
using MyForms.Models;
using MyForms.Models.VM.FormVMs;
using MyForms.Models.VM.TemplateVMs;
using System.Security.Claims;

namespace MyForms.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext db;


        public ProfileController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
        }

        public async Task<IActionResult> MyTemplates()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var templates = await db.Templates
                .Where(t => t.CreatedById == userId)
                .ToListAsync();
            var templateVMs = templates.Select(x => new TemplateVM()
            {
                Template = x
            }).ToList();
            return View(templateVMs);
        }

        public async Task<IActionResult> MyForms()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var forms = await db.Forms
                .Include(f => f.Template)
                .Where(f => f.FilledBy == userId)
                .ToListAsync();
            var formVMs = forms.Select(x => new FormVM()
            {
                Form = x
            }).ToList();
            return View(formVMs);
        }
    }
}
