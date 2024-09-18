namespace OnlineStore.Web.ViewModels.Products;

public class ProductColorServiceModel
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;

    public int ColorId { get; set; }
    public string ColorName {  get; set; } = string.Empty;
}
