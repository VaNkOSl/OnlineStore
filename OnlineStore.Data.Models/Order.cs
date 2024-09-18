namespace OnlineStore.Data.Models;

using OnlineStore.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static OnlineStore.Commons.EntityValidationConstraints.ApplicationUsers;
using static OnlineStore.Commons.EntityValidationConstraints.Orders;
public class Order
{
    public Order()
    {
        Id = Guid.NewGuid();
        OrderItems = new HashSet<OrderItem>();
        ProductImages = new HashSet<ProductImage>();
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(UserFirstNameMaxLength)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(UserLastNameMaxLength)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(OrderAdressMaxLength)]
    public string Adress {  get; set; } = string.Empty;

    [Required]
    [MaxLength(UserPhoneNumberMaxLenght)]
    public string PhoneNumber {  get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DateTime CreateOrderdDate { get; set; }
    public DateTime? ShippedDate { get; set; }

    [Required]
    public DeliveryOption DeliveryOption { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public bool IsTaken { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; }
    public virtual ICollection<ProductImage> ProductImages { get; set; }
}
