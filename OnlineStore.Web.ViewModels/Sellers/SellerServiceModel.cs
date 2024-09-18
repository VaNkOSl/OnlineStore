namespace OnlineStore.Web.ViewModels.Sellers;

using System.ComponentModel.DataAnnotations;
public class SellerServiceModel
{
    public string Id { get; set; } = string.Empty;
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;
    [Display(Name = "Phone number")]
    public string PhoneNumber {  get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
