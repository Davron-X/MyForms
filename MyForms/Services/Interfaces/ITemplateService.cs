using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyForms.Services.Interfaces
{
    public interface ITemplateService
    {
        public Task<List<SelectListItem>> GetTopicSelectListAsync();
        public Task AssignUserAccessAsync(IEnumerable<string> emails,int templateId);
        public Task AssignTagsAsync(IEnumerable<string> tags, int templateId);
    }
}
