using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MyForms.Data;
using MyForms.Models;
using MyForms.Models.VM.Form;
using MyForms.Models.VM.FormVMs;
using MyForms.Models.VM.TemplateVMs;
using System.Security.Claims;

namespace MyForms.Services.Interfaces
{
    public interface IFormService
    {
        Task<List<FormVM>> GetAllFormsAsync();

        Task RemoveFormsAsync(IEnumerable<int> formIds);

        Task<CreateVM?> PrepareFormCreationAsync(int templateId);

        Task<bool> SaveFormAsync(CreateVM createVM, string userId);

        Task<OverviewVM?> GetFormOverviewAsync(int formId, ClaimsPrincipal user);

        Task<bool> UpdateFormAnswersAsync(OverviewVM overviewVM);
    }

   
}
