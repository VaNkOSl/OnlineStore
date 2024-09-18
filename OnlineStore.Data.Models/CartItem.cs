namespace OnlineStore.Data.Models;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
public class CartItem
{
    public CartItem()
    {
        Id = Guid.NewGuid();
        SelectedColor = new List<string>();
        SelectedSizes = new List<string>();
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;

    [Required]
    public Guid ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public List<string> SelectedColor { get; set; } 
    public List<string> SelectedSizes { get; set; }
}
