namespace OnlineStore.Web.ViewModels.Admin;

public class CategoryPreDeleteViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> ProductNames = new List<string>();
}
