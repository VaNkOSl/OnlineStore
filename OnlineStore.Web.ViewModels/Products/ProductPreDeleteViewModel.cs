namespace OnlineStore.Web.ViewModels.Products;

public class ProductPreDeleteViewModel
{
    public ProductPreDeleteViewModel()
    {
        ProductImages = new HashSet<ProductImageServiceModel>();
    }

    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Image { get; set; } = string.Empty;
    public ICollection<ProductImageServiceModel> ProductImages { get; set; }
}
