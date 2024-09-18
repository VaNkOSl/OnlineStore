namespace OnlineStore.Data.Models;

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static OnlineStore.Commons.EntityValidationConstraints.ApplicationUsers;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        Id = Guid.NewGuid();
        Products = new HashSet<Product>();
        Orders = new HashSet<Order>();
        Reviews = new HashSet<Review>();
        Notifications = new HashSet<Notification>();
    }

    [Required]
    [MaxLength(UserFirstNameMaxLength)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(UserLastNameMaxLength)]
    public string LastName { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }

    public Seller? Seller { get; set; }
    public virtual ICollection<Product> Products { get; set; }
    public virtual ICollection<Order> Orders { get; set; }
    public virtual ICollection<Review> Reviews { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; }
}
