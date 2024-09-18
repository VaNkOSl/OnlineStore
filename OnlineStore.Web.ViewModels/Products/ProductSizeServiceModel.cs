namespace OnlineStore.Web.ViewModels.Products;

public class ProductSizeServiceModel
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;

    public int SizeId { get; set; }
    public string SizeName { get; set; } = string.Empty;
}
