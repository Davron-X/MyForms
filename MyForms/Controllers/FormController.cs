using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MyForms.Data;
using MyForms.Filters;
using MyForms.Models;
using MyForms.Models.VM.Form;
using MyForms.Models.VM.FormVMs;
using MyForms.Models.VM.TemplateVMs;
using MyForms.Services.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyForms.Controllers
{
    [Authorize]
    public class FormController : Controller
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IFormService formService;

        public FormController(ApplicationDbContext db, IStringLocalizer<SharedResource> localizer, IFormService formService)
        {
            _localizer = localizer;
            this.formService = formService;
        }
        [Authorize(Roles =AppConsts.AdminRole)]
        public async Task<IActionResult> Index()
        {
            var forms=await formService.GetAllFormsAsync();
            return View(forms);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(IEnumerable<FormVM> formVMs)
        {
            var formsToRemove = formVMs.Where(x => x.IsChecked).Select(x => x.Form.Id).ToList();
            await formService.RemoveFormsAsync(formsToRemove);
            return RedirectToAction("Index", "Home");


        }

        [TypeFilter(typeof(TemplateAccessFilter))]
        public async Task<IActionResult> Create(int templateId)
        {
            
            var createVM = await formService.PrepareFormCreationAsync(templateId);
            if (createVM is null)
            {
                return NotFound();
            }
            return View(createVM);
        }

        [HttpPost]
        [TypeFilter(typeof(TemplateAccessFilter))]
        public async Task<IActionResult> Create(CreateVM createVM,int templateId)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", _localizer["AnswerError"]);
               
                createVM = await formService.PrepareFormCreationAsync(templateId);
                return View(createVM);
            }
            string? userId= User.FindFirstValue(ClaimTypes.NameIdentifier);
            await formService.SaveFormAsync(createVM, userId);
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> Overview(int formId)
        {
            var overviewVM = await formService.GetFormOverviewAsync(formId, User);               
            if (overviewVM is null)
            {
                return NotFound();
            }          
            return View(overviewVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(OverviewVM overviewVM)
        {
            bool result =await formService.UpdateFormAnswersAsync(overviewVM);
            if (!result)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Index", "Home");


        }
    }
}
