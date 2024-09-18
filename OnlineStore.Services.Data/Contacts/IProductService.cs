namespace OnlineStore.Services.Data.Contacts;

using Microsoft.AspNetCore.Http;
using OnlineStore.Data.Models;
using OnlineStore.Web.ViewModels.Admin;
using OnlineStore.Web.ViewModels.Products;
using OnlineStore.Web.ViewModels.Reviews;

public interface IProductService
{
    Task<IEnumerable<ProductBrandServiceModel>> AllBrandsAsync();
    Task<IEnumerable<ProductCategoryServiceModel>> AllCategoriesAsync();
    Task<IEnumerable<ProductColorServiceModel>> AllColorsAsync();
    Task<IEnumerable<ProductSizeServiceModel>> AllSizesAsync();
    Task<string> CreateProductAsync(ProductFormModel model, string sellerId, List<int> selectedColors,List<int> selectedSizes, IFormFileCollection images);
    Task<bool> CategoryExistsAsync(int categoryId);
    Task<bool> BrandExistsAsync(int brandId);
    Task<bool> ColorExistsAsync(IEnumerable<int> colorId);
    Task<bool> SizeExistsAsync(IEnumerable<int> sizeIds);
    Task<ProductQueryServiceModel> AllProductsAsync(AllProductsQueryModel model);
    Task<ProductDetailsViewModel> GetProductDetailsByIdAsync(string productId);
    Task<bool> ProductExistsAsync(string productId);
    Task<string> CreateProductReviewAsync(ProductReviewViewModel model, string userId);
    Task<IEnumerable<ProductServiceModel>> AllProductsBySellerIdAsync(string sellerId);
    Task<ProductPreDeleteViewModel> GetProductForDeletingByIdAsync(string productId);
    Task DeleteProductByIdAsync(string productId);
    Task DeteleProductSizesByIdAsync(string productId);
    Task DeleteProductColorsByIdAsync(string productId);
    Task DeleteProductImagesAsync(string productId);
    Task DeleteProductReviewAsync(string productId);
    Task<bool> ProductHasReviewAsync(string productId);
    Task<ProductFormModel> GetProductForEditByIdAsync(string productId);
    Task EditProductByFormModelAsync(ProductFormModel model,string productId, List<int> selectedColorIds, List<int> selectedSizeIds,IFormFileCollection images);
    Task<IEnumerable<ProductIndexServiceModel>> LastProductsAsync();
    Task ApplyDiscountAsync(string productId, int discountPercentage);
    Task<Product> GetProductByIdAsync(string productId);
    Task<bool> ProductHasOrdersAsync(string productId);
    Task DeleteProductCartItemsByIdAsync(string productId);
    Task<string> GetProductNameAsync(string productId);
    Task<List<ProductForApproveServiceModel>> GetUnapprovedProductsAsync();
    Task ApproveProductAsync(string productId);
    Task RejectProductAsync(RejectProductFormModel model,string productId);
}
