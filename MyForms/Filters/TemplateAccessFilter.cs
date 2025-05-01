using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MyForms.Data;
using System.Security.Claims;

namespace MyForms.Filters
{
    public class TemplateAccessFilter : IAsyncActionFilter
    {
        private readonly ApplicationDbContext db;
        public TemplateAccessFilter(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if ( actionDescriptor?.ActionName == "Index")
            {
                await next();
                return;
            }
            context.ActionArguments.TryGetValue("templateId",out object? id);
            var templateId = id as int?;
            if (templateId is null)
            {
                context.Result = new ForbidResult();
                return;
            }
            var template = await db.Templates.Include(x=>x.TemplateUsers).FirstOrDefaultAsync(x => x.Id == templateId);
            if (template is null)
            {
                context.Result = new ForbidResult();
                return;
            }
            var user=context.HttpContext.User;
            string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier);           
            if (template.IsPrivate)
            {
                if (userId is null)
                {
                    context.Result = new ForbidResult();
                    return;
                }
                if (user.IsInRole(AppConsts.AdminRole) ||
                    template.CreatedById==userId||
                        template.TemplateUsers.Any(x=>x.UserId==userId))
                {
                    await next();
                    return;
                }
                context.Result = new ForbidResult();
                return;
                
            }
            await next();

        }
    }
}
