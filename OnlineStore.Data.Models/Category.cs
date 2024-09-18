namespace OnlineStore.Data.Models;

using System.ComponentModel.DataAnnotations;
using static OnlineStore.Commons.EntityValidationConstraints.Categoryes;
public class Category
{
    public Category()
    {
        Products = new HashSet<Product>();
    }

    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(CategoryNameMaxLength)]
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<Product> Products { get; set; }

}
