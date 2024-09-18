namespace OnlineStore.Services.Data.Contacts;

public interface IProductAttributeService
{
    Task<IEnumerable<string>> AllBrandsNameAsync();
    Task<IEnumerable<string>> AllCategoriesNamesAsync();
    Task<IEnumerable<string>> AllSizesNameAsync();
    Task<IEnumerable<string>> AllColorsNameAsync();
}
