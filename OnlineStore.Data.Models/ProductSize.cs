namespace OnlineStore.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class ProductSize
{

    public ProductSize()
    {
        Id = Guid.NewGuid();
    }
    [Key]
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;

    public int SizeId { get; set; }

    [ForeignKey(nameof(SizeId))]
    public virtual Size Size { get; set; } = null!;
}
