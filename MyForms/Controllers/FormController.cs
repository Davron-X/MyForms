using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyForms.Data;
using MyForms.Models;
using MyForms.Models.VM.Form;
using MyForms.Models.VM.FormVMs;
using MyForms.Models.VM.TemplateVMs;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyForms.Controllers
{
    [Authorize]
    public class FormController : Controller
    {
        private readonly ApplicationDbContext db;

        public FormController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            var forms=await db.Forms.Include(x=>x.Template).Include(x=>x.ApplicationUser).Select(x=>new FormVM() { Form = x }).ToListAsync();
            return View(forms);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(IEnumerable<FormVM> formVMs)
        {
            var formsToRemove = formVMs.Where(x => x.IsChecked).Select(x => x.Form.Id).ToList();
            await db.Forms.Where(x => formsToRemove.Contains(x.Id)).ExecuteDeleteAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create(int templateId)
        {
            var template= db.Templates.Include(x=>x.Questions).ThenInclude(x=>x.AnswerOptions)
                .FirstOrDefault(x => x.Id == templateId);
            if (template is null)
            {
                return NotFound();
            }
            CreateVM createVM = new()
            {
                Template = template,
                FormAnswers=template.Questions.Select(x=>new FormAnswer() { QuestionId=x.Id}).ToList()
            };
            return View(createVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateVM createVM)
        {
            if (!ModelState.IsValid)
            {
                return View(createVM);
            }
            Form form = new()
            {
                FilledBy = User.FindFirstValue(ClaimTypes.NameIdentifier)!,                
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
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Overview(int formId)
        {
           
            var form= await db.Forms.Include(x=>x.FormAnswers)
                .Include(x=>x.Template).ThenInclude(x=>x.Questions).ThenInclude(x=>x.AnswerOptions)
                .FirstOrDefaultAsync(x => x.Id == formId);           
            if (form is null)
            {
                return NotFound();
            }
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != form.FilledBy && form?.Template?.CreatedById != userId && !User.IsInRole(AppConsts.AdminRole))
            {
                return Forbid();
            }
            OverviewVM overviewVM = new()
            {
                Form = form,
                IsReadOnly= !User.IsInRole(AppConsts.AdminRole) && User.FindFirstValue(ClaimTypes.NameIdentifier) != form.FilledBy
            };
            return View(overviewVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(OverviewVM overviewVM)
        {
            if (overviewVM.IsReadOnly)
            {
                return RedirectToAction(nameof(Index));
            }
            db.FormAnswers.UpdateRange(overviewVM.Form.FormAnswers);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
