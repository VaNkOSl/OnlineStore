namespace OnlineStore.Web.ViewModels.Products;

using OnlineStore.Data.Models.Enums;
using OnlineStore.Web.ViewModels.Reviews;
using OnlineStore.Web.ViewModels.Sellers;

public class ProductDetailsViewModel : ProductServiceModel
{
    public ProductDetailsViewModel()
    {
        Reviews = new List<ProductReviewViewModel>();
        Colors = new List<string>();
        Sizes = new List<string>();
        ImagePaths = new List<string>();
    }
    public string Description { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public bool IsAvaible { get; set; }
    public Season SuitableSeason { get; set; }
    public SellerServiceModel Seller { get; set; } = null!;
    public List<ProductReviewViewModel> Reviews { get; set; }
    public List<string> Colors { get; set; }
    public List<string> Sizes { get; set; }
    public List<string> ImagePaths { get; set; }
}
