namespace OnlineStore.Web.ViewModels.Orders;

using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using static OnlineStore.Commons.EntityValidationConstraints.ApplicationUsers;
using static OnlineStore.Commons.EntityValidationConstraints.Orders;
public class OrderFormModel
{
    public OrderFormModel()
    {
        Images = new HashSet<ProductImage>();
    }

    [Required]
    [StringLength(UserFirstNameMaxLength,MinimumLength = UserFirstNameMinLength)]
    [Display(Name = "Enter your first name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(UserLastNameMaxLength,MinimumLength = UserLastNameMinLength)]
    [Display(Name = "Enter your lasst name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(OrderAdressMaxLength,MinimumLength = OrderAdressMinLength)]
    public string Adress { get; set; } = string.Empty;

    [Required]
    [StringLength(UserPhoneNumberMaxLenght,MinimumLength = UserPhoneNumberMinLenght)]
    public string PhoneNumber {  get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DeliveryOption DeliveryOption { get; set; }
    public ICollection<ProductImage> Images { get; set; }
}
