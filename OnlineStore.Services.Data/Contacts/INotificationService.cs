namespace OnlineStore.Services.Data.Contacts;

using OnlineStore.Web.ViewModels.Notification;
public interface INotificationService
{
    Task CreateMessegeAsync(NotificationFormModel model,string userId);
    Task CreateResponseSellerAsync(NotificationResponseFormModel model, string userId);
    Task<IEnumerable<NotificationViewModel>> GetNotificationsByUserIdAsync(string userId);
    Task CreateResponseUserAsync(NotificationResponseFormModel model,string userId);
    Task<IEnumerable<NotificationSellerViewModel>> GetNotificationsBySellerIdAsync(string sellerId,string userId, bool isAdmin);
    Task MarkAsRespondedBySellerAsync(string notificationId);
    Task MarkAsRespondedByAdminAsync(string notificationId);
    Task MarkAsRespondedByUserAsync(string notificationId);
    Task DeleteSellerRequestAsync(string id);
    Task<NotificationResponseFormModel> GetNotificationResponseModelAsync(string id);
}
