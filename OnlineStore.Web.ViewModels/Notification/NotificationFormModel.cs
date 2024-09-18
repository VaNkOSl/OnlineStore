namespace OnlineStore.Web.ViewModels.Notification;

using System.ComponentModel.DataAnnotations;
using static OnlineStore.Commons.EntityValidationConstraints.Notifications;

public class NotificationFormModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;

    [StringLength(NotificationsMessegerMaxLength, MinimumLength = NotificationsMessegerMinLength)]
    [Display(Name = "Enter messege to send")]
    public string Message { get; set; } = string.Empty;

    [EmailAddress]
    [Display(Name = "Enter email")]
    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
