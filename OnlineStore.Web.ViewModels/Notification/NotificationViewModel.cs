namespace OnlineStore.Web.ViewModels.Notification;

public class NotificationViewModel
{
    public string Id { get; set; } = string.Empty; 
    public string Message { get; set; } = string.Empty; 
    public string Response { get; set; } = string.Empty; 
    public DateTime? DateSenden { get; set; }  
    public DateTime? DateResponded { get; set; }  
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserId {  get; set; } = string.Empty;
}
