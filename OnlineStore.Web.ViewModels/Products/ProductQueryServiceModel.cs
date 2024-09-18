namespace OnlineStore.Web.ViewModels.Products;

public class ProductQueryServiceModel
{
    public ProductQueryServiceModel()
    {
        Products = new HashSet<ProductServiceModel>();
    }
    public int TotalProductsCount { get; set; }
    public IEnumerable<ProductServiceModel> Products { get; set; }
}
