namespace OnlineStore.Web.ViewModels.Admin;

using OnlineStore.Web.ViewModels.Sellers;
public class ProductForApproveServiceModel
{
    public string Id { get; set; } = string.Empty;
    public string Name {  get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string FirstImage { get; set; } = string.Empty;
    public string CategoryName {  get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public int StockQuantity { get; set; } 
    public SellerServiceModel Seller { get; set; } = null!;
    public List<string> Colors { get; set; } = new List<string>();
    public List<string> Sizes { get; set; }  = new List<string>();
}
