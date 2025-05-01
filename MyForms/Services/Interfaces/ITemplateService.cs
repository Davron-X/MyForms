using Microsoft.AspNetCore.Mvc.Rendering;
using MyForms.Models;
using MyForms.Models.VM.TemplateVMs;
using System.Linq.Expressions;

namespace MyForms.Services.Interfaces
{
    public interface ITemplateService
    {
        public Task<List<Template>> GetAllTemplatesAsync(Expression<Func<Template, bool>>? filter = null, bool includeTopic = false, bool includeAuthor = false);

        public Task<List<Question>> GetQuestionsAsync(Expression<Func<Question, bool>>? filter = null, bool includeAnswerOptions = false);

        public Task<Template?> GetTemplateAsync(Expression<Func<Template, bool>>? filter = null, bool includeFormsAndUser = false,
            bool includeTemplateUsersAndUser = false, bool includeTemplateTagsAndTag = false);

        public Task RemoveTemplatesAsync(IEnumerable<int> templateIds);

        public Task<TemplateStatisticsVM?> GetTemplateStatisticsAsync(int templateId);

        public Task<TemplateDetailsVM?> GetTemplateDetailsAsync(int templateId,string userId);

        public Task CreateAsync(TemplateCreateVM templateCreateVM, string userId);

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
