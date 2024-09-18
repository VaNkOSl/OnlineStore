namespace OnlineStore.Data.Models;

using System.ComponentModel.DataAnnotations;
using static OnlineStore.Commons.EntityValidationConstraints.Notifications;
public class Notification
{
    public Notification()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }

    [MaxLength(NotificationsMessegerMaxLength)]
    public string? Messege { get; set; }

    [MaxLength(NotificationResponseMaxLength)]
    public string? Response { get; set; }
    public bool IsReadedByUser { get; set; }
    public bool IsReadedBySeller { get; set; }
    public bool IsReadedByAdmin { get; set; }
    public Guid? UserId { get; set; }
    public ApplicationUser User { get; set; }
    public Guid? SellerId { get; set; }
    public Guid SenderId { get; set; }
    public Seller Seller { get; set; }
    public DateTime? DateSenden { get; set; }
    public DateTime? DateResponded { get; set; }
}
