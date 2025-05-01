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


namespace MyForms.Services
{
    public class HomeService : IHomeService
    {
        private readonly ApplicationDbContext _db;

        public HomeService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<HomeVM> GetHomeViewModelAsync()
        {
            var recentTemplates = await _db.Templates
                .Include(x => x.Likes)
                .Include(x => x.Forms)
                .Include(x => x.ApplicationUser)
                .Where(x => !x.IsPrivate)
                .OrderBy(x => x.CreatedAt)
                .Take(5)
                .ToListAsync();

            var popularTemplates = await _db.Templates
                .Include(x => x.Forms)
                .Include(x => x.TemplateTags).ThenInclude(x => x.Tag)
                .Include(x => x.ApplicationUser)
                .Where(x => !x.IsPrivate)
                .OrderByDescending(x => x.Forms.Count)
                .Take(10)
                .ToListAsync();

            var tags = await _db.Tags
                .Include(x => x.TemplateTags)
                .OrderByDescending(x => x.TemplateTags.Count)
                .Take(10)
                .Select(x => new TagVM { Tag = x, Count = x.TemplateTags.Count })
                .ToListAsync();

            return new HomeVM
            {
                RecentTemplates = recentTemplates,
                PopularTemplates = popularTemplates,
                Tags = tags
            };
        }

        public List<TemplateSearchResult> SearchTemplates(string query)
        {
            return _db.Templates
                .Where(t => EF.Functions.Contains(t.Title, query)
                         || EF.Functions.Contains(t.Description, query)
                         || t.Questions.Any(q => EF.Functions.Contains(q.Text, query))
                         || t.Comments.Any(c => EF.Functions.Contains(c.Text, query))
                         || t.TemplateTags.Any(tt => EF.Functions.Contains(tt.Tag.Name, query)))
                .Select(t => new TemplateSearchResult
                {
                    Id = t.Id,
                    Title = t.Title,
                    Snippet = EF.Functions.Contains(t.Description, query)
                        ? t.Description
                        : t.Questions.FirstOrDefault(q => EF.Functions.Contains(q.Text, query)).Text
                        ?? t.Comments.FirstOrDefault(c => EF.Functions.Contains(c.Text, query)).Text
                }) 
                .Take(10)
                .ToList();
        }
    }

}
