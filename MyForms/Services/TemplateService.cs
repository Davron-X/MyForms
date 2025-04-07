using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyForms.Data;
using MyForms.Models.VM;
using MyForms.Models;
using MyForms.Services.Interfaces;

namespace MyForms.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly ApplicationDbContext db;

        public TemplateService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task AssignTagsAsync(IEnumerable<string> tags, int templateId)
        {
            List<Tag> existingTags = await db.Tags.Where(x => tags.Contains(x.Name)).ToListAsync();
            var newTagNames = tags.Except(existingTags.Select(x=>x.Name)).ToList();
            var newTags = newTagNames.Select(x => new Tag() { Name = x }).ToList();
            if (newTags.Any())
            {
                await db.Tags.AddRangeAsync(newTags);
                await db.SaveChangesAsync();
            }
            existingTags= existingTags.Concat(newTags).ToList();
            List<TemplateTag> templateTags = existingTags.Select(x => new TemplateTag()
            {
                TemplateId = templateId,
                TagId = x.Id
            }).ToList();
            await db.TemplateTags.AddRangeAsync(templateTags);
            await db.SaveChangesAsync();
        }

        public async Task AssignUserAccessAsync(IEnumerable<string> emails, int templateId)
        {
            var userIds = await db.ApplicationUsers.Where(x => emails.Contains(x.Email)).Select(x => x.Id).ToListAsync();
            List<TemplateUserAccess> userAccesses = userIds.Select(x=>new TemplateUserAccess()
            {
                UserId=x,
                TemplateId=templateId
            }).ToList();           
            await db.TemplateUserAccesses.AddRangeAsync(userAccesses);
            await db.SaveChangesAsync();
        }

        public async Task<List<SelectListItem>> GetTopicSelectListAsync()           
        {
           return await db.Topics.Select(x => new SelectListItem(x.Name, x.Id.ToString(), x.Id == 1)).ToListAsync();
        }
    }
}
