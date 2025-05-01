using Microsoft.Extensions.Localization;
using MyForms.Data;
using MyForms.Models.VM.Form;
using MyForms.Models.VM.FormVMs;
using MyForms.Models;
using MyForms.Services.Interfaces;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace MyForms.Services
{
    public class FormService : IFormService
    {
        private readonly ApplicationDbContext db;

        private readonly IStringLocalizer<SharedResource> _localizer;

        public FormService(ApplicationDbContext db, IStringLocalizer<SharedResource> localizer)
        {
            this.db = db;
            _localizer = localizer;
        }

        public async Task<List<FormVM>> GetAllFormsAsync()
        {
           return await db.Forms.Include(x => x.Template).Include(x => x.ApplicationUser).Select(x => new FormVM() { Form = x }).ToListAsync();
        }

        public async Task RemoveFormsAsync(IEnumerable<int> formIds)
        {
            await db.Forms.Where(x => formIds.Contains(x.Id)).ExecuteDeleteAsync();
        }

        public async Task<CreateVM?> PrepareFormCreationAsync(int templateId)
        {
            var template = await db.Templates
                .Include(x => x.Questions).ThenInclude(x => x.AnswerOptions)
                .FirstOrDefaultAsync(x => x.Id == templateId);

            if (template is null) return null;

            return new CreateVM
            {
                Template = template,
                FormAnswers = template.Questions.Select(x => new FormAnswer { QuestionId = x.Id }).ToList()
            };
        }

        public async Task<bool> SaveFormAsync(CreateVM createVM, string userId)
        {
            if (!createVM.FormAnswers.Any()) return false;

            Form form = new()
            {
                FilledBy = userId,
                TemplateId = createVM.Template!.Id
            };
            await db.Forms.AddAsync(form);
            await db.SaveChangesAsync();

            foreach (var answer in createVM.FormAnswers)
            {
                answer.FormId = form.Id;
            }
            db.FormAnswers.AddRange(createVM.FormAnswers);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<OverviewVM?> GetFormOverviewAsync(int formId, ClaimsPrincipal user)
        {
            var form = await db.Forms
                .Include(x => x.FormAnswers)
                .Include(x => x.Template).ThenInclude(x => x.Questions).ThenInclude(x => x.AnswerOptions)
                .FirstOrDefaultAsync(x => x.Id == formId);

            if (form is null) return null;

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != form.FilledBy && form.Template?.CreatedById != userId && !user.IsInRole(AppConsts.AdminRole))
            {
                return null;
            }

            return new OverviewVM
            {
                Form = form,
                IsReadOnly = !user.IsInRole(AppConsts.AdminRole) && userId != form.FilledBy
            };
        }

        public async Task<bool> UpdateFormAnswersAsync(OverviewVM overviewVM)
        {
            if (overviewVM.IsReadOnly) return false;
            db.FormAnswers.UpdateRange(overviewVM.Form.FormAnswers);
            await db.SaveChangesAsync();
            return true;
        }
    }
}
