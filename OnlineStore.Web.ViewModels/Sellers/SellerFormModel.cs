namespace OnlineStore.Web.ViewModels.Sellers;

using System.ComponentModel.DataAnnotations;
using static OnlineStore.Commons.EntityValidationConstraints.Sellers;
public class SellerFormModel
{
    [Required]
    [StringLength(SellerFirstNameMaxLength,MinimumLength = SellrFirstNameMinLength)]
    [Display(Name = "Enter your first name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(SellerLastNameMaxLength,MinimumLength = SellerLastNameMinLength)]
    [Display(Name = "Enter your last name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(SellerDescriptionMaxLength,MinimumLength = SellerDescriptionMinLength)]
    [Display(Name = "Enter some information about you")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(SellerPhoneNumberMaxLenght,MinimumLength = SellerPhoneNumberMinLenght)]
    [Display(Name = "Enter your phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [StringLength(SellerEgnMaxLength,MinimumLength = SellerEgnMinLength)]
    [Display(Name = "Enter your valid egn")]
    public string Egn { get; set; } = string.Empty;
}
