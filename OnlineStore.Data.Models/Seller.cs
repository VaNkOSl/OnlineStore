namespace OnlineStore.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static OnlineStore.Commons.EntityValidationConstraints.Sellers;

public class Seller
{
    public Seller()
    {
        Id = Guid.NewGuid();
        OwnedProducts = new HashSet<Product>();
        Notifications = new HashSet<Notification>();
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(SellerFirstNameMaxLength)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(SellerLastNameMaxLength)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(SellerDescriptionMaxLength)]
    public string Description {  get; set; } = string.Empty;

    [Required]
    [MaxLength(SellerPhoneNumberMaxLenght)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }
    public bool IsApproved {  get; set; }
    public bool IsAdminReject { get; set; }

    [Required]
    [MaxLength(SellerEgnMaxLength)]
    public string Egn { get; set; } = string.Empty;

    [MaxLength(SellerRejectionReasonMaxLength)]
    public string? RejectionReason { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ICollection<Product> OwnedProducts { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; }

}
//public Seller(
//string egn,
//DateTime dateOfBirth,
//string firstName,
//string lastName,
//string phoneNumber,
//Guid userId,
//string description)
//{
//    Egn = egn;
//    DateOfBirth = dateOfBirth;
//    FirstName = firstName;
//    LastName = lastName;
//    PhoneNumber = phoneNumber;
//    UserId = userId;
//    Description = description;
//    IsApproved = false; // Default value
//}