namespace OnlineStore.Web.ViewModels.Products;

using System.ComponentModel.DataAnnotations;
public class ApplyDiscountViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100.")]
    public int DiscountPercentage { get; set; }
}
