namespace OnlineStore.Services.Data.Contacts;

using OnlineStore.Web.ViewModels.Admin;
using OnlineStore.Web.ViewModels.Sellers;
public interface ISellerService
{
    Task<bool> ExistsByIdAsync(string userId);
    Task<string> GetSellerByIdAsync(string userId);
    Task CreateSellerAsync(SellerFormModel model, string userId);
    Task<bool> SellerWithEgnAlredyExistsAsync(string egn);
    Task<bool> SellerWithPhoneNumberAlredyExistsAsync(string phoneNumber);
    Task<bool> SellerHasProductsAsync(string userId,string productId);
    Task<List<SellerForReviewViewModel>> GetUnapprovedSellersAsync();
    Task ApproveSellerAsync(string sellerId);
    Task RejectSellerAsync(RejectSellerFormModel model, string sellerId);
    Task<bool> IsUserApprovedAsync(string userId);
    Task<string> GetSellerByNameAsync(string sellerId);
    Task<string> GetRejectionReasonAsync(string userId);
    Task<bool> IsAdminRejectedAsync (string userId);
    Task<bool> SellerEmailExistsAsync(string userId, string email);
}
