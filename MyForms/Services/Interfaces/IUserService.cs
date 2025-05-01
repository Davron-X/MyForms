using MyForms.Models.VM.User;

namespace MyForms.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<ApplicationUserVM>> GetAllUsersWithRolesAsync();

        Task BlockUsersAsync(IEnumerable<string> userIds);

        Task UnblockUsersAsync(IEnumerable<string> userIds);

        Task RemoveUsersAsync(IEnumerable<string> userIds);

        Task AddToAdminRoleAsync(IEnumerable<string> userIds);

        Task RemoveFromAdminRoleAsync(IEnumerable<string> userIds);
    }
}
