namespace OnlineStore.Services.Data;

using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Data.Common;
using OnlineStore.Data.Models;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.Admin;
using OnlineStore.Web.ViewModels.Sellers;
using System.Collections.Generic;

public class SellerService : ISellerService
{
    private readonly IRepository repository;

    public SellerService(IRepository _repository)
    {
        repository = _repository;  
    }

    public async Task CreateSellerAsync(SellerFormModel model, string userId)
    {
        Seller seller = new Seller
        {
          Egn = model.Egn,
          DateOfBirth = model.DateOfBirth,
          FirstName = model.FirstName,
          LastName = model.LastName,
          IsApproved = false,
          PhoneNumber = model.PhoneNumber,
          UserId = Guid.Parse(userId),
          Description = model.Description,
        };

        await repository.AddAsync(seller);
        await repository.SaveChangesAsync();
    }

    public async Task<bool> ExistsByIdAsync(string userId)
    {
       return await repository
            .AllReadOnly<Seller>()
            .AnyAsync(s => s.UserId.ToString() == userId);
    }

    public async Task<string> GetSellerByIdAsync(string userId)
    {
        Seller? seller = await repository
            .AllReadOnly<Seller>()
            .FirstOrDefaultAsync(s => s.UserId.ToString() == userId && s.IsApproved);

        if(seller == null)
        {
            return null!;
        }

        return seller.Id.ToString();
    }

    public async Task<List<SellerForReviewViewModel>> GetUnapprovedSellersAsync()
    {
        return await repository
            .AllReadOnly<Seller>()
            .Where(s => !s.IsApproved && string.IsNullOrEmpty(s.RejectionReason))
            .Select(s => new SellerForReviewViewModel
            {
                Id = s.Id.ToString(),
                DateOfBirth = s.DateOfBirth,
                Description = s.Description,
                Egn = s.Egn,
                Email = s.User.Email ?? "Unknown email",
                FullName = $"{s.FirstName} {s.LastName}",
                PhoneNumber = s.PhoneNumber,
            }).ToListAsync();
    }

    public async Task ApproveSellerAsync(string sellerId)
    {
        Guid sellerGuidId = Guid.Parse(sellerId);

        var seller = await repository
            .GetByIdAsync<Seller>(sellerGuidId);

        if(seller != null && !seller.IsApproved)
        {
            seller.IsApproved = true;
            await repository.SaveChangesAsync();
        }
    }
    public async Task RejectSellerAsync(RejectSellerFormModel model, string sellerId)
    {
        Guid sellerGuidId = Guid.Parse(sellerId);

        var seller = await repository
            .GetByIdAsync<Seller>(sellerGuidId);

        if (seller != null && !seller.IsApproved)
        {
            seller.IsApproved = false;
            seller.IsAdminReject = true;
            seller.RejectionReason = model.RejectionReason;
            await repository.SaveChangesAsync();
        }
    }

    public async Task<bool> SellerHasProductsAsync(string userId, string productId)
    {
        var seller = await repository
            .AllReadOnly<Seller>()
            .Include(s => s.OwnedProducts)
            .FirstOrDefaultAsync(s => s.UserId.ToString() == userId);

        if (seller == null)
        {
            return false;
        }

        return seller.OwnedProducts.Any(op => op.Id.ToString().ToLower() == productId.ToLower());
    }

    public async Task<bool> SellerWithEgnAlredyExistsAsync(string egn)
    {
        return await repository
            .AllReadOnly<Seller>()
            .AnyAsync(s => s.Egn == egn);
    }

    public async Task<bool> SellerWithPhoneNumberAlredyExistsAsync(string phoneNumber)
    {
        return await repository
          .AllReadOnly<Seller>()
          .AnyAsync(s => s.PhoneNumber == phoneNumber);
    }

    public async Task<bool> IsUserApprovedAsync(string userId)
    {
        return await repository
            .AllReadOnly<Seller>()
            .AnyAsync(s => s.UserId.ToString() == userId && s.IsApproved == true);
    }

    public async Task<string> GetSellerByNameAsync(string sellerId)
    {
        var seller = await repository
            .AllReadOnly<Seller>()
            .FirstOrDefaultAsync(s => s.Id.ToString() == sellerId);

        if (seller == null)
        {
            return "Unknown seller name";
        }

        return $"{seller!.FirstName} {seller.LastName}";
    }

    public async Task<string> GetRejectionReasonAsync(string userId)
    {
        var seller = await repository
             .AllReadOnly<Seller>()
             .FirstOrDefaultAsync(s => s.UserId.ToString() == userId);

        if(seller is null)
        {
            return "No reason for your rejecting provided!";
        }

        return seller.RejectionReason ?? string.Empty;
    }

    public async Task<bool> IsAdminRejectedAsync(string userId)
    {
        return await repository
             .AllReadOnly<Seller>()
             .AnyAsync(s => s.UserId.ToString() == userId && s.IsAdminReject == true);
    }

    public async Task<bool> SellerEmailExistsAsync(string userId,string email)
    {
        var user = await repository
          .AllReadOnly<ApplicationUser>()
          .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return false;
        }

        var sellerExists = await repository
            .AllReadOnly<Seller>()
            .AnyAsync(s => s.UserId == user.Id);

        return sellerExists;
    }
}
