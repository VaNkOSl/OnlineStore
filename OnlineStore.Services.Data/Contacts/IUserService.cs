namespace OnlineStore.Services.Data.Contacts;

using OnlineStore.Data.Models;
using OnlineStore.Web.ViewModels.Admin;
public interface IUserService
{
    Task<string> GetUserFullNameAsync(string userId);
    Task<IEnumerable<UserViewModel> > GetAllUsersAsync();
    Task BlockUserAsync(string userId);
    Task<IEnumerable<BlockUserViewModel>> GetUserToBlockAsync(string userId);
    Task<bool> UserExistsAsync(string userId);
    Task<bool> UserEmailExistsAsync(string email);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
}
