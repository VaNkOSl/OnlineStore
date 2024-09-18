using OnlineStore.Data.Models;

namespace OnlineStore.Web.ViewModels.ShopingCart;

public class CartItemViewModel
{
    public CartItemViewModel()
    {
        SelectedColors = new List<string>();
        SelectedSizes = new List<string>();
        AvaibleColors = new List<string>();
        AvaibleSizes = new List<string>();
    }
    public string Id { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }
    public List<string> SelectedColors { get; set; }
    public List<string> SelectedSizes { get; set; }
    public List<string> AvaibleColors { get; set; }
    public List<string> AvaibleSizes { get; set; }
}
