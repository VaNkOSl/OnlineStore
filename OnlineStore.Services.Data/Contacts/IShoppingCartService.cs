namespace OnlineStore.Services.Data.Contacts;

using OnlineStore.Web.ViewModels.ShopingCart;
public interface IShoppingCartService
{
    Task AddToCartAsync(string userId, string productId, int quantity, List<string> selectedColor, List<string> selectedSizes);
    Task RemoveFromCartAsync(string cartItemId);
    Task UpdateQuantityCartAsync(string cartItemId, int quantity);
    Task UpdateOrderItemQuantityAsync(string cartItemId, int quantity);
    Task<IEnumerable<CartItemViewModel>> GetAllCartItemAsync(string userId);
    Task<string> GetProductNameAsync(string cartItemId);
    Task UpdateColorsAsync(string cartItemId, List<string> colors);
    Task UpdateSizesAsync(string cartItemId, List<string> sizes);
    Task UpdateOrderItemsColorsAsync(string cartItemId, List<string> colors);
    Task UpdateOrderItemsSizesAsync(string cartItemId, List<string> sizes);
    Task<bool> UserHasItemsInCart(string userId);
    Task ClearCartItemsAsync(); 
    Task<bool> ProductAlreadyExistInCartItemAsync(string productId,string userId);
}
