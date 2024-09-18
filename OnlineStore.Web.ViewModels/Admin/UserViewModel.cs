namespace OnlineStore.Web.ViewModels.Admin;

using System.ComponentModel.DataAnnotations;

public class UserViewModel
{
    public string Id { get; set; } = string.Empty;

    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsSeller { get; set; }
    public string Email { get; set; } = string.Empty;
}
