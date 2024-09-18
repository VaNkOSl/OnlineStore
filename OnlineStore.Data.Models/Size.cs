namespace OnlineStore.Data.Models;

using System.ComponentModel.DataAnnotations;
using static OnlineStore.Commons.EntityValidationConstraints.Sizes;
public class Size
{
    public Size()
    {
        Products = new HashSet<Product>();
    }

    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(SizeNameMaxLength)]
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<Product> Products { get; set; }
}
