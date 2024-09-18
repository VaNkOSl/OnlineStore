namespace OnlineStore.Data.Models;

using System.ComponentModel.DataAnnotations;
using static OnlineStore.Commons.EntityValidationConstraints.Brands;
public class Brand
{
    public Brand()
    {
        Products = new HashSet<Product>();
    }

    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(BrandNameMaxLength)]
    public string Name { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public virtual ICollection<Product> Products { get; set; }
}
