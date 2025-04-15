using Microsoft.AspNetCore.Mvc.Rendering;
using MyForms.Models;

namespace MyForms.Services.Interfaces
{
    public interface ITemplateService
    {
        public Task<List<SelectListItem>> GetTopicSelectListAsync();
        public Task AssignUserAccessAsync(IEnumerable<string> emails,int templateId);
        public Task AssignTagsAsync(IEnumerable<string> tags, int templateId);
        public Task UpdateTagsAsync(IEnumerable<string> tagNames, int templateId);
        public Task UpdateTemplateAsync(Template template);
        public Task UpdateUserAccessAsync(IEnumerable<string> emails, int templateId);
        public Task AddQuestions(IEnumerable<Question> questions, int templateId);
        public Task UpdateQuestionsAsync(IEnumerable<Question> questions, int templateId);
        public  Task<string> UploadImageAsync(IFormFile file);

    }
}
