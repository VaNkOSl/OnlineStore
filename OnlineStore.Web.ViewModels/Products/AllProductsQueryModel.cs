namespace OnlineStore.Web.ViewModels.Products;

using OnlineStore.Web.ViewModels.Products.Enums;
using System.ComponentModel.DataAnnotations;
using static OnlineStore.Commons.GeneralApplicationConstants;
public class AllProductsQueryModel
{
    public AllProductsQueryModel()
    {
        CurrentPage = DefaultPage;
        ProductPerPage = EntitiesPerPage;

        Categories = new HashSet<string>();
        Brands = new HashSet<string>();
        Colors = new HashSet<string>();
        Sizes = new HashSet<string>();
        Products = new HashSet<ProductServiceModel>();
    }

    [Display(Name = "Search by Category")]
    public string? Category {  get; set; }

    [Display(Name = "Search by Brand")]
    public string? Brand { get; set; }

    [Display(Name = "Search by Color")]
    public string? Color { get; set; }

    [Display(Name = "Search by Size")]
    public string? Size { get; set; }
    public int TotalProducs { get; set; }

    public ProductSorting ProductSorting { get; set; }
    public string? SearchinString { get; set; }
    public int CurrentPage { get; set; }
    public int ProductPerPage { get; set; }

    public IEnumerable<string> Categories { get; set; }
    public IEnumerable<string> Brands { get; set; }
    public IEnumerable<string> Colors { get; set; }
    public IEnumerable<string> Sizes { get; set; }
    public IEnumerable<ProductServiceModel> Products { get; set; }
}
