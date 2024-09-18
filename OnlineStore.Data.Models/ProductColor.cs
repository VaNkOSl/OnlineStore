namespace OnlineStore.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProductColor
{
    public ProductColor()
    {
        Id = Guid.NewGuid();
    }

    [Key]
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;

    public int ColorId { get; set; }

    [ForeignKey(nameof(ColorId))]
    public virtual Color Color { get; set; } = null!;
}
