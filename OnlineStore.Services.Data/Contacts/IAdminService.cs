namespace OnlineStore.Services.Data.Contacts;

using OnlineStore.Web.ViewModels.Admin;
public interface IAdminService
{
    Task CreateColorAsync(ColorFormModel model, string adminId);
    Task<bool> ColorNameExistAsync(string colorName);
    Task<bool> ColorExistByIdAsync(int colorId);
    Task<List<ColorPreDeleteViewModel>> GetColorPreDeleteByIdAsync(int colorId);
    Task DeleteColorAsync(int colorId);
    Task CreateBrandAsync(BrandFormModel model,string adminId);
    Task<bool> BrandNameExistAsync(string brandName);
    Task<bool> BrandExistByIdAsync(int brandId);
    Task<List<BrandPreDeleteViewModel>> GetBrandPreDeleteByIdAsync(int brandId);
    Task DeleteBrandAsync(int brandId);
    Task CreateSizeAsync(SizeFormModel model,string adminId);
    Task<bool> SizeNameExistAsync(string sizeName);
    Task<bool> SizeExistByIdAsync(int sizeId);
    Task<List<SizePreDeleteViewModel>> GetSizePreDeleteByIdAsync(int sizeId);
    Task DeleteSizeAsync(int sizeId);
    Task CreateCategoryAsync(CategoryFormModel model,string adminId);
    Task<bool> CategoryNameExistAsync(string categoryName);
    Task<bool> CategoryExistByIdAsync(int categoryId);
    Task<List<CategoryPreDeleteViewModel>> GetCategoryPreDeleteByIdAsync(int categoryId);
    Task DeleteCategoryAsync(int categoryId);
}
