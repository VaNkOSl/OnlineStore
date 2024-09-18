namespace OnlineStore.Data.Models;

using OnlineStore.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static OnlineStore.Commons.EntityValidationConstraints.Products;
public class Product
{
    public Product()
    {
        Id = Guid.NewGuid();
        ProductImages = new HashSet<ProductImage>();
        AvailableColors = new HashSet<ProductColor>();
        AvailableSizes = new HashSet<ProductSize>();
        Reviews = new HashSet<Review>();
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(ProductNameMaxLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(ProductDescriptionMaxLength)]
    public string Description {  get; set; } = string.Empty;

    [Required]
    [Range(ProductStockQuantityMinValue, ProductStockQuantityMaxValue)]
    public int StockQuantity { get; set; }

    [MaxLength(ProductRejectionReasonMaxLength)]
    public string? RejectionReason {  get; set; }

    [Required]
    public DateTime DateAdded { get; set; }

    [Required]
    public decimal Price { get; set; }
    public bool IsAvaible { get; set; }
    public bool IsApproved { get; set; }
    public Season SuitableSeason { get; set; }

    [Required]
    public Guid SellerId { get; set; }

    [ForeignKey(nameof(SellerId))]
    public virtual Seller Seller { get; set; } = null!;

    [Required]
    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public virtual Category Category { get; set; } = null!;

    [Required]
    public int BrandId { get; set; }

    [ForeignKey(nameof(BrandId))]
    public virtual Brand Brand { get; set; } = null!;
    public virtual ICollection<ProductImage> ProductImages { get; set; }
    public virtual ICollection<ProductColor> AvailableColors { get; set; }
    public virtual ICollection<ProductSize> AvailableSizes { get; set; }
    public virtual ICollection<Review> Reviews { get; set; }
}