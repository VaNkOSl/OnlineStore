namespace OnlineStore.Data.Models;

using System.ComponentModel.DataAnnotations;
using static OnlineStore.Commons.EntityValidationConstraints.Colors;
public class Color
{
    public Color()
    {
        Products = new HashSet<Product>();
    }

    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(ColorNameMaxLength)]
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<Product> Products { get; set; }
}
