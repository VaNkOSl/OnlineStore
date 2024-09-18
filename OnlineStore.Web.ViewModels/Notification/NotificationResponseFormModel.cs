namespace OnlineStore.Web.ViewModels.Notification;

using System.ComponentModel.DataAnnotations;
using static OnlineStore.Commons.EntityValidationConstraints.Notifications;

public class NotificationResponseFormModel
{
    public string Id { get; set; } = string.Empty;

    [StringLength(NotificationResponseMaxLength, MinimumLength = NotificationResponseMinLength)]
    [Display(Name = "Enter response to send")]
    public string ResponseMessage { get; set; } = string.Empty;
    public string UserFullName {  get; set; } = string.Empty;
    public DateTime ResponseAt { get; set; }
}
