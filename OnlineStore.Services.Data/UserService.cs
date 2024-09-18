namespace OnlineStore.Services.Data;

using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Data.Common;
using OnlineStore.Data.Models;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.Admin;
using System.Collections.Generic;

public class UserService : IUserService
{
    private readonly IRepository repository;

    public UserService(IRepository _repository)
    {
        repository = _repository;
    }

    public async Task BlockUserAsync(string userId)
    {
        Guid userIdGuid = Guid.Parse(userId);

        var userToBlock = await repository
            .GetByIdAsync<ApplicationUser>(userIdGuid);

        if(userToBlock != null)
        {
            userToBlock.IsBlocked = true;
            await repository.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<UserViewModel>> GetAllUsersAsync()
    {
        List<UserViewModel> allUsers = await repository
            .AllReadOnly<ApplicationUser>()
            .Where(ap => ap.IsBlocked == false)
            .Select(u => new UserViewModel()
            {
                Id = u.Id.ToString(),
                Email = u.Email ?? string.Empty,
                FullName = u.FirstName + " " + u.LastName
            }).ToListAsync();

        foreach(UserViewModel user in allUsers)
        {
            Seller? seller = await repository
                .AllReadOnly<Seller>()
                .FirstOrDefaultAsync(s => s.UserId.ToString() == user.Id);

            if(seller != null)
            {
                user.PhoneNumber = seller.PhoneNumber;
                user.IsSeller = true;
            }
            else
            {
                user.PhoneNumber = string.Empty;
            }
        }

        return allUsers;
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await repository
            .AllReadOnly<ApplicationUser>()
            .FirstOrDefaultAsync(ap => ap.Email == email);
    }

    public async Task<string> GetUserFullNameAsync(string userId)
    {
        var user = await repository
            .AllReadOnly<ApplicationUser>()
            .FirstOrDefaultAsync(ap => ap.Id.ToString() == userId);

        if(user == null)
        {
           return "Unknown user";
        }

        return $"{user.FirstName} {user.LastName}";
    }

    public async Task<IEnumerable<BlockUserViewModel>> GetUserToBlockAsync(string userId)
    {
        return await repository
            .AllReadOnly<ApplicationUser>()
            .Where(u => u.Id.ToString() == userId)
            .Select(ap => new BlockUserViewModel
            {
                Id = ap.Id.ToString(),
                Email = ap.Email ?? string.Empty,
                UserFullName = $"{ap.FirstName} {ap.LastName}"
            }).ToListAsync();
    }

    public async Task<bool> UserEmailExistsAsync(string email)
    {
        return await repository
            .AllReadOnly<ApplicationUser>()
            .AnyAsync(ap => ap.Email == email);
    }

    public async Task<bool> UserExistsAsync(string userId)
    {
        return await repository
            .AllReadOnly<ApplicationUser>()
            .AnyAsync(ap => ap.Id.ToString() == userId);
    }
}
