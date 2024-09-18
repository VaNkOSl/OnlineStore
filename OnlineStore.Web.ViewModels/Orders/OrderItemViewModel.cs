namespace OnlineStore.Web.ViewModels.Orders;

using OnlineStore.Web.ViewModels.Products;
public class OrderItemViewModel
{
    public OrderItemViewModel()
    {
        ProductImages = new HashSet<ProductImageServiceModel>();
        SelectedColors = new List<string>();
        SelectedSizes = new List<string>();
    }

    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> SelectedColors { get; set; } 
    public List<string> SelectedSizes { get; set; } 
    public ICollection<ProductImageServiceModel> ProductImages { get; set; }
}
