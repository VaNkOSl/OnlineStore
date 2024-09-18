namespace OnlineStore.Web.ViewModels.Products;

public class ProductServiceModel
{

    public ProductServiceModel()
    {
        Images = new HashSet<ProductImageServiceModel>();
    }
    public string Id { get; set; } = string.Empty;
    public string Name {  get; set; } = string.Empty;
    public decimal Price {  get; set; }
    public string FirstImageUrl { get; set; } = string.Empty;
    public bool IsAvaible { get; set; }
    public bool IsApproved { get; set; }
    public string RejectionReason { get; set; }
    public IEnumerable<ProductImageServiceModel> Images { get; set; }
}
