using MyForms.Models.VM.HomeVMs;
using MyForms.Models;

namespace MyForms.Services.Interfaces
{
    public interface IHomeService
    {
        Task<HomeVM> GetHomeViewModelAsync();

        List<TemplateSearchResult> SearchTemplates(string query);
    }
}
