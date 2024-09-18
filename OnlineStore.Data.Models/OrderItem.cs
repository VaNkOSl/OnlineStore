namespace OnlineStore.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static OnlineStore.Commons.EntityValidationConstraints.OrderItems;
public class OrderItem
{
    public OrderItem()
    {
        Id = Guid.NewGuid();
        SelectedColor = new List<string>();
        SelectedSizes = new List<string>();
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid OrderId { get; set; }

    [ForeignKey(nameof(OrderId))]
    public virtual Order Order { get; set; } = null!;

    [Required]
    public Guid ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;

    [Required]
    [MaxLength(OrderItemQuantityMaxLength)]
    public int Quantity { get; set; }

    [Required]
    public decimal Price { get; set; }
    public List<string> SelectedColor { get; set; }
    public List<string> SelectedSizes { get; set; }
}