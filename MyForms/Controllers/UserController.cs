using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyForms.Data;
using MyForms.Models;
using MyForms.Models.VM.User;
using MyForms.Services.Interfaces;

namespace MyForms.Controllers
{
    [Authorize(Roles =AppConsts.AdminRole)]
    public class UserController : Controller
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await userService.GetAllUsersWithRolesAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> BlockUsers(IEnumerable<ApplicationUserVM> userVMs)
        {
            var ids = userVMs.Where(x => x.IsSelected && !x.IsBlocked).Select(x => x.Id);
            await userService.BlockUsersAsync(ids);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UnBlockUsers(IEnumerable<ApplicationUserVM> userVMs)
        {
            var ids = userVMs.Where(x => x.IsSelected && x.IsBlocked).Select(x => x.Id);
            await userService.UnblockUsersAsync(ids);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUsers(IEnumerable<ApplicationUserVM> userVMs)
        {
            var ids = userVMs.Where(x => x.IsSelected).Select(x => x.Id);
            await userService.RemoveUsersAsync(ids);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddToAdmins(IEnumerable<ApplicationUserVM> userVMs)
        {
            var ids = userVMs.Where(x => x.IsSelected && !x.Roles.Contains(AppConsts.AdminRole)).Select(x => x.Id);
            await userService.AddToAdminRoleAsync(ids);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAdminRoles(IEnumerable<ApplicationUserVM> userVMs)
        {
            var ids = userVMs.Where(x => x.IsSelected && x.Roles.Contains(AppConsts.AdminRole)).Select(x => x.Id);
            await userService.RemoveFromAdminRoleAsync(ids);
            return RedirectToAction(nameof(Index));
        }
    }
}
