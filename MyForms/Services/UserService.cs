using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyForms.Data;
using MyForms.Models.VM.User;
using MyForms.Services.Interfaces;

namespace MyForms.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext db;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserService(ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.roleManager = roleManager;
        }

        public async Task<List<ApplicationUserVM>> GetAllUsersWithRolesAsync()
        {
            var userVMs = await db.ApplicationUsers.Select(x => new ApplicationUserVM
            {
                Id = x.Id,
                Email = x.Email,
                FullName = x.FullName,
                IsBlocked = x.LockoutEnd > DateTime.UtcNow
            }).ToListAsync();

            var userRoles = await db.UserRoles.Where(ur => userVMs.Select(u => u.Id).Contains(ur.UserId))
                .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, Role = r.Name })
                .GroupBy(x => x.UserId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.Role));

            foreach (var user in userVMs)
            {
                if (userRoles.TryGetValue(user.Id, out var roles))
                {
                    user.Roles = roles.ToList();
                }
            }

            return userVMs;
        }

        public async Task BlockUsersAsync(IEnumerable<string> userIds)
        {
            await db.ApplicationUsers.Where(x => userIds.Contains(x.Id))
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.LockoutEnd, DateTime.UtcNow.AddYears(100)));
        }

        public async Task UnblockUsersAsync(IEnumerable<string> userIds)
        {
            await db.ApplicationUsers.Where(x => userIds.Contains(x.Id))
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.LockoutEnd, default(DateTimeOffset?)));
        }

        public async Task RemoveUsersAsync(IEnumerable<string> userIds)
        {
            await db.ApplicationUsers.Where(x => userIds.Contains(x.Id)).ExecuteDeleteAsync();
        }

        public async Task AddToAdminRoleAsync(IEnumerable<string> userIds)
        {
            var adminRole = await roleManager.FindByNameAsync(AppConsts.AdminRole);
            var userRoles = userIds.Select(x => new IdentityUserRole<string>
            {
                UserId = x,
                RoleId = adminRole.Id
            });
            await db.UserRoles.AddRangeAsync(userRoles);
            await db.SaveChangesAsync();
        }

        public async Task RemoveFromAdminRoleAsync(IEnumerable<string> userIds)
        {
            var adminRole = await roleManager.FindByNameAsync(AppConsts.AdminRole);
            await db.UserRoles.Where(x => userIds.Contains(x.UserId) && x.RoleId == adminRole.Id)
                .ExecuteDeleteAsync();
        }
    }
}
