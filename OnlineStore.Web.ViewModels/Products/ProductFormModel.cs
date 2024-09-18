namespace OnlineStore.Web.ViewModels.Products;

using Microsoft.AspNetCore.Http;
using OnlineStore.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using static OnlineStore.Commons.EntityValidationConstraints.Products;
public class ProductFormModel
{
    public ProductFormModel()
    {
        Brands = new HashSet<ProductBrandServiceModel>();
        Categories = new HashSet<ProductCategoryServiceModel>();
        Colors = new HashSet<ProductColorServiceModel>();
        Sizes = new HashSet<ProductSizeServiceModel>();
        Images = new HashSet<IFormFile>();
    }

    [Required]
    [StringLength(ProductNameMaxLength,MinimumLength = ProductNameMinLength)]
    [Display(Name = "Enter a product name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(ProductDescriptionMaxLength,MinimumLength =ProductDescriptionMinLength)]
    [Display(Name = "Enter a product descriptions")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(ProductStockQuantityMinValue, ProductStockQuantityMaxValue)]
    [Display(Name = "Enter a quantity of the product")]
    public int StockQuantity { get; set; }

    [Required]
    [Range(typeof(decimal), ProductPriceMinValue, ProductPriceMaxValue)]
    [Display(Name = "Enter a product price")]
    public decimal Price { get; set; }

    [Required]
    [Display(Name = "Select Suitable Season")]
    public Season SuitableSeason { get; set; }

    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required]
    [Display(Name = "Brand")]
    public int BrandId { get; set; }

    public  IEnumerable<ProductBrandServiceModel> Brands { get; set; }
    public  IEnumerable<ProductCategoryServiceModel> Categories { get; set; }
    public IEnumerable<ProductColorServiceModel> Colors { get; set; }
    public IEnumerable<ProductSizeServiceModel> Sizes { get; set; }
    public virtual ICollection<IFormFile> Images { get; set; }
}
