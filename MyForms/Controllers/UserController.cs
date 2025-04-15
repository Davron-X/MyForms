using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyForms.Data;
using MyForms.Models;
using MyForms.Models.VM.User;

namespace MyForms.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var userVMs= await db.ApplicationUsers.Select(x => new ApplicationUserVM()
            {
                Id=x.Id,
                Email = x.Email,
                FullName = x.FullName,
                IsBlocked=x.LockoutEnd > DateTime.UtcNow            
            }).ToListAsync();
            var userRoles= await db.UserRoles.Where(ur => userVMs.Select(u => u.Id).Contains(ur.UserId))
                .Join(db.Roles, ur => ur.RoleId, r => r.Id,
                (ur, r) => new
                {
                    userId = ur.UserId,
                    role = r.Name
                }).GroupBy(x=>x.userId)
                .ToDictionaryAsync(k=>k.Key,v=>v.Select(x=>x.role));

            foreach (var user in userVMs)
            {
                if (userRoles.TryGetValue(user.Id,out var roles))
                {
                    user.Roles = roles.ToList();
                }
            }
            return View(userVMs);
        }

        [HttpPost]
        public async Task<IActionResult> BlockUsers(IEnumerable<ApplicationUserVM> userVMs)
        {
            var usersToBlock= userVMs.Where(x => x.IsSelected && !x.IsBlocked).Select(x => x.Id).ToList();
            if (!usersToBlock.Any())
            {
                return RedirectToAction(nameof(Index));
            }
            await db.ApplicationUsers.Where(x => usersToBlock.Contains(x.Id))
                .ExecuteUpdateAsync(x=>x.SetProperty(p=>p.LockoutEnd,DateTime.UtcNow.AddYears(100)));
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UnBlockUsers(IEnumerable<ApplicationUserVM> userVMs)
        {
            var usersToUnblock = userVMs.Where(x => x.IsSelected && x.IsBlocked).Select(x => x.Id);
            if (!usersToUnblock.Any())
            {
                return RedirectToAction(nameof(Index));
            }
            await db.ApplicationUsers.Where(x => usersToUnblock.Contains(x.Id))
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.LockoutEnd, default(DateTimeOffset?)));
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUsers(IEnumerable<ApplicationUserVM> userVMs)
        {
            var usersToRemove= userVMs.Where(x => x.IsSelected).Select(x => x.Id);
            if (!usersToRemove.Any())
            {
                return RedirectToAction(nameof(Index));
            }
            await db.ApplicationUsers.Where(x=>usersToRemove.Contains(x.Id)).ExecuteDeleteAsync();
         
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> AddToAdmins(IEnumerable<ApplicationUserVM> userVMs)
        {
            var userIds = userVMs.Where(x => x.IsSelected &&  !x.Roles.Contains(AppConsts.AdminRole)).Select(x => x.Id);
            if (!userIds.Any())
            {
                return RedirectToAction(nameof(Index));
            }
            var adminRole=await roleManager.FindByNameAsync(AppConsts.AdminRole);
            var userRoles= userIds.Select(x => new IdentityUserRole<string>()
            {
                UserId = x,
                RoleId = adminRole.Id
            });
            await db.UserRoles.AddRangeAsync(userRoles);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAdminRoles(IEnumerable<ApplicationUserVM> userVMs)
        {
            var userIds = userVMs.Where(x => x.IsSelected && x.Roles.Contains(AppConsts.AdminRole)).Select(x => x.Id);
            if (!userIds.Any())
            {
                return RedirectToAction(nameof(Index));
            }
            var adminRole=await roleManager.FindByNameAsync(AppConsts.AdminRole);
            await db.UserRoles.Where(x=>userIds.Contains(x.UserId) && x.RoleId==adminRole.Id).ExecuteDeleteAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
