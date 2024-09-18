namespace OnlineStore.Web.ViewModels.Admin;

public class BrandPreDeleteViewModel
{
    public BrandPreDeleteViewModel()
    {
        ProductNames = new List<string>();
    }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> ProductNames { get; set; }
}
