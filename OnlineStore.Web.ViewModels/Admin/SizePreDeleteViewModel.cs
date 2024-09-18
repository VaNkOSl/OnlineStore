namespace OnlineStore.Web.ViewModels.Admin;

public class SizePreDeleteViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> ProductNames { get; set; } = new List<string>();
}
