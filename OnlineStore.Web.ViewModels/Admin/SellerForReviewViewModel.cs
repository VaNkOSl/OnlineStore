using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Web.ViewModels.Admin;
public class SellerForReviewViewModel
{
    public string Id { get; set; } = string.Empty;
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Egn { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}
