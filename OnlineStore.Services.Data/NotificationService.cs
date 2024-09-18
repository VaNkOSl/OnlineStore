namespace OnlineStore.Services.Data;

using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Data.Common;
using OnlineStore.Data.Models;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.Notification;


public class NotificationService : INotificationService
{
    private readonly IRepository repository;

    public NotificationService(IRepository _repository)
    {
        repository = _repository; 
    }
    public async Task CreateMessegeAsync(NotificationFormModel model, string userId)
    {
        var recipientUser = await repository
              .All<ApplicationUser>()
              .Include(u => u.Seller)
              .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (recipientUser == null)
        {
            throw new InvalidOperationException($"Cannot find user with email {model.Email}");
        }

        Guid? recipientSellerId = recipientUser.Seller?.Id;

        var senderId = Guid.Parse(userId);
        var sender = await repository
            .AllReadOnly<Seller>()
            .FirstOrDefaultAsync(s => s.UserId == senderId);

        Notification notification = new Notification
        {
            DateSenden = DateTime.Now,
            Messege = model.Message ?? throw new ArgumentNullException(nameof(model.Message)),
            UserId = recipientUser.Id,
            SenderId = Guid.Parse(userId),
            SellerId = recipientSellerId ?? sender?.Id 
        };

        recipientUser.Notifications.Add(notification);

        if (recipientSellerId.HasValue)
        {
            var recipientSeller = await repository
                .All<Seller>()
                .FirstOrDefaultAsync(s => s.Id == recipientSellerId.Value);

            if (recipientSeller != null)
            {
                recipientSeller.Notifications.Add(notification);
            }
        }

        await repository.AddAsync(notification);
        await repository.SaveChangesAsync();
    }

    public async Task CreateResponseSellerAsync(NotificationResponseFormModel model, string userId)
    {
        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            throw new ArgumentException("Invalid user ID.");
        }

        var notification = await repository
            .All<Notification>()
            .Include(s => s.Seller)
            .Include(u => u.User)
            .FirstOrDefaultAsync(n => n.Id.ToString() == model.Id);

        if (notification == null)
        {
            throw new InvalidOperationException($"Notification with ID {model.Id} not found.");
        }

        notification.Response = model.ResponseMessage;
        notification.DateResponded = DateTime.Now;

        var userFullName = $"{notification.User.FirstName} {notification.User.LastName}";
        model.UserFullName = userFullName;

        await repository.UpdateAsync(notification);
        await repository.SaveChangesAsync();
    }

    public async Task CreateResponseUserAsync(NotificationResponseFormModel model, string userId)
    {
        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            throw new ArgumentException("Invalid user ID.");
        }

        var notification = await repository
            .All<Notification>()
            .Include(u => u.User)
            .FirstOrDefaultAsync(n => n.Id.ToString() == model.Id);

        if (notification == null)
        {
            throw new InvalidOperationException($"Notification with ID {model.Id} not found.");
        }

        notification.Response = model.ResponseMessage;
        notification.DateResponded = DateTime.Now;
        var userFullName = $"{notification.User.FirstName} {notification.User.LastName}";
        model.UserFullName = userFullName;

        await repository.UpdateAsync(notification);
        await repository.SaveChangesAsync();
    }

    public async Task DeleteSellerRequestAsync(string id)
    {
        var seller = await repository
            .All<Seller>()
            .Where(s => s.Id.ToString() == id)
            .ToListAsync();

        if(seller.Any())
        {
            await repository.DeleteRange(seller);
            await repository.SaveChangesAsync();
        }
    }

    public async Task<NotificationResponseFormModel> GetNotificationResponseModelAsync(string id)
    {
        Guid notificatioIdToGuid = Guid.Parse(id);

        var notification = await
            repository.GetByIdAsync<Notification>(notificatioIdToGuid);

        if(notification != null)
        {
            return new NotificationResponseFormModel { Id = id };
        }

        return null!;
    }

    public async Task<IEnumerable<NotificationSellerViewModel>> GetNotificationsBySellerIdAsync(string sellerId,string userId,bool isAdmin)
    {
        var sellerGuid = Guid.Parse(sellerId);
        var userGuid = Guid.Parse(userId);

        var notificationsQuery = repository
            .AllReadOnly<Notification>()
            .Include(n => n.User) 
            .Include(n => n.Seller) 
            .ThenInclude(s => s.User) 
            .Where(n => n.UserId.ToString() == userId && n.SellerId.ToString() == sellerId);

        if (isAdmin)
        {
            notificationsQuery = notificationsQuery.Where(n => !n.IsReadedByAdmin);
        }
        else
        {
            notificationsQuery = notificationsQuery.Where(n => !n.IsReadedBySeller);
        }

        var notifications = await notificationsQuery
            .Select(n => new NotificationSellerViewModel
            {
                Id = n.Id.ToString(),
                Message = n.Messege ?? string.Empty,
                Response = n.Response ?? string.Empty,
                DateSenden = n.DateSenden,
                DateResponded = n.DateResponded,

                UserFullName = n.SenderId != userGuid
                    ? $"{n.User.FirstName} {n.User.LastName}"
                    : (n.Seller != null && n.Seller.User != null
                        ? $"{n.Seller.User.FirstName} {n.Seller.User.LastName}"
                        : "Unknown"),

                UserEmail = n.SenderId == userGuid
                    ? n.User.Email 
                    : (n.Seller != null && n.Seller.User != null
                        ? n.Seller.User.Email 
                        : "Unknown")
            })
            .ToListAsync();

        return notifications;
    }

    public async Task<IEnumerable<NotificationViewModel>> GetNotificationsByUserIdAsync(string userId)
    {
        var userGuid = Guid.Parse(userId);

        var notifications = await repository
            .AllReadOnly<Notification>()
            .Include(n => n.User)
            .Include(n => n.Seller)
            .Where(n =>
                (n.UserId == userGuid && !n.IsReadedByUser) ||
                (n.SenderId == userGuid)
            )
            .Select(n => new NotificationViewModel
            {
                Id = n.Id.ToString(),
                UserId = userId,
                Message = n.Messege ?? string.Empty,
                Response = n.Response ?? string.Empty,
                DateSenden = n.DateSenden,
                DateResponded = n.DateResponded,

                UserName = n.SenderId == userGuid
                    ? $"{n.User.FirstName} {n.User.LastName}"
                    : (n.Seller != null && n.Seller.User != null
                        ? $"{n.Seller.User.FirstName} {n.Seller.User.LastName}"
                        : $"{n.User.FirstName} {n.User.LastName}"),

                UserEmail = n.SenderId == userGuid
                    ? n.User.Email
                    : (n.Seller != null && n.Seller.User != null
                        ? n.Seller.User.Email
                        : n.User.Email)
            })
            .ToListAsync();

        return notifications;
    }

  

    public async Task MarkAsRespondedByAdminAsync(string notificationId)
    {
        var notification = await repository
            .All<Notification>()
            .FirstOrDefaultAsync(n => n.Id.ToString() == notificationId);

        if (notification == null)
        {
            return;
        }

        notification.IsReadedByAdmin = true;
        await repository.UpdateAsync(notification);
        await repository.SaveChangesAsync();
    }

    public async Task MarkAsRespondedBySellerAsync(string notificationId)
    {
       var notification = await repository
            .All<Notification>()
            .FirstOrDefaultAsync(n => n.Id.ToString() == notificationId);

        if(notification == null)
        {
            return;
        }

        notification.IsReadedBySeller = true;
        await repository.UpdateAsync(notification);
        await repository.SaveChangesAsync();
    }

    public async Task MarkAsRespondedByUserAsync(string notificationId)
    {
        var notification = await repository
            .All<Notification>()
            .FirstOrDefaultAsync(n => n.Id.ToString() == notificationId);

        if (notification == null)
        {
            return;
        }

        notification.IsReadedByUser = true;
        await repository.UpdateAsync(notification);
        await repository.SaveChangesAsync();
    }
}
