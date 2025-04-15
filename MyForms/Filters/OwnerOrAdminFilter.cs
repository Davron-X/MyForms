using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MyForms.Data;
using System.Security.Claims;

namespace MyForms.Filters
{
    public class OwnerOrAdminFilter : IAsyncActionFilter
    {
        private readonly ApplicationDbContext db;
        public OwnerOrAdminFilter( ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            int? templateId= context.ActionArguments["templateId"] as int? ;
            if (templateId is null)
            {
                context.Result = new ForbidResult();
                return;
            }
            string? userId= context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var template= await db.Templates.FirstOrDefaultAsync(x => x.Id == templateId);
            if (userId is null || template is null)
            {
                context.Result = new ForbidResult();
                return;
            }
            if (!context.HttpContext.User.IsInRole(AppConsts.AdminRole) && template.CreatedById!=userId)
            {
                context.Result = new ForbidResult();
                return;
            }
            await next();
        }
    }
}
