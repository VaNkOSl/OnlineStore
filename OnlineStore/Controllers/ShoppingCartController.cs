namespace OnlineStore.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.Infrastructure.Extensions;
using static OnlineStore.Commons.MessagesConstants;
using static OnlineStore.Commons.NotificationMessagesConstants;

[Authorize]
public class ShoppingCartController : BaseController
{
    private readonly IShoppingCartService shoppingCartService;
    private readonly ISellerService sellerService;
    private readonly IProductService productService;

    public ShoppingCartController(IShoppingCartService _shoppingCartService, ISellerService _sellerService,
                                IProductService _productService)
    {
        shoppingCartService = _shoppingCartService;
        sellerService = _sellerService;
        productService = _productService;
    }

    [HttpGet]
    public async Task<IActionResult> CartItem()
    {
        try
        {
            var userId = User.GetId()!;

            var model = await shoppingCartService.GetAllCartItemAsync(userId);
            return View(model);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(string productId,int quantity, List<string> SelectedColor, List<string> SelectedSizes)
    {
        var userId = User.GetId()!;

        if (!SelectedColor.Any())
        {
            TempData[ErrorMessage] = UserDoesNotSelectAColor;
            return RedirectToAction(nameof(ProductController.Details), "Product", new { id = productId });
        }

        if (!SelectedSizes.Any())
        {
            TempData[ErrorMessage] = UserDoesNotSelectASize;
            return RedirectToAction(nameof(ProductController.Details), "Product", new { id = productId });
        }

        if (await sellerService.ExistsByIdAsync(userId) == true)
        {
            TempData[ErrorMessage] = SellersCannonMakeOrders;
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

         var productName = await productService.GetProductNameAsync(productId);

        if (await shoppingCartService.ProductAlreadyExistInCartItemAsync(productId, userId) == true)
        {
            TempData[WarningMessage] = string.Format(ProductAlreadyAddedToCartItem,productName);
            return RedirectToAction("CartItem", "ShoppingCart");  
        }

        try
        {
            await shoppingCartService.AddToCartAsync(userId, productId, quantity, SelectedColor, SelectedSizes);
            TempData[SuccessMessage] = string.Format(SuccessfullyAddedItemToCart, productName);
            return RedirectToAction(nameof(ProductController.Details), "Product", new { id = productId });
        }
        catch (Exception)
        {
            TempData[ErrorMessage] = string.Format(UnexpectedErrorOccurredAddToCartItem, productName);
            return RedirectToAction(nameof(ProductController.Details), "Product", new { id = productId });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(string cartItemId)
    {
        if (await sellerService.ExistsByIdAsync(User.GetId()!) == true)
        {
            TempData[ErrorMessage] = SellersCannonMakeOrders;
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        var productName = await shoppingCartService.GetProductNameAsync(cartItemId);

        try
        {
            await shoppingCartService.RemoveFromCartAsync(cartItemId);
            TempData[WarningMessage] = string.Format(SuccessfullyDeleteProductFromCart, productName);
            return RedirectToAction("CartItem");
        }
        catch (Exception)
        {
            TempData[ErrorMessage] = string.Format(UnexpectedErrorOccurredRemoveFromCart, productName);
            return RedirectToAction("CartItem", "ShoppingCart");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateColors(string cartItemId, List<string> colors)
    {
        if (await sellerService.ExistsByIdAsync(User.GetId()!) == true)
        {
            TempData[ErrorMessage] = SellersCannonMakeOrders;
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        if (!colors.Any())
        {
            TempData[ErrorMessage] = UserDoesNotSelectAColor;
            return RedirectToAction("CartItem", "ShoppingCart");
        }

        var productName = await shoppingCartService.GetProductNameAsync(cartItemId);

        try
        {
            await shoppingCartService.UpdateColorsAsync(cartItemId, colors);
            await shoppingCartService.UpdateOrderItemsColorsAsync(cartItemId, colors);
            return RedirectToAction("CartItem");
        }
        catch (Exception)
        {
            TempData[ErrorMessage] = string.Format(UnexpectedErrorOccurredUpdateColor, productName);
            return RedirectToAction("CartItem", "ShoppingCart");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSizes(string cartItemId, List<string> sizes)
    {
        if (await sellerService.ExistsByIdAsync(User.GetId()!) == true)
        {
            TempData[ErrorMessage] = SellersCannonMakeOrders;
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        if (!sizes.Any())
        {
            TempData[ErrorMessage] = UserDoesNotSelectASize;
            return RedirectToAction("CartItem", "ShoppingCart");
        }

        var productName = await shoppingCartService.GetProductNameAsync(cartItemId);

        try
        {
            await shoppingCartService.UpdateSizesAsync(cartItemId, sizes);
            await shoppingCartService.UpdateOrderItemsSizesAsync(cartItemId, sizes);
            return RedirectToAction("CartItem");
        }
        catch (Exception)
        {
           TempData[ErrorMessage] = string.Format(UnexpectedErrorOccurredUpdateSize, productName);
            return RedirectToAction("CartItem", "ShoppingCart");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(string cartItemId, int quantity)
    {
        if (await sellerService.ExistsByIdAsync(User.GetId()!) == true)
        {
            TempData[ErrorMessage] = SellersCannonMakeOrders;
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        if (quantity <= 0)
        {
            TempData[ErrorMessage] = QuantityMustBePositiveNumber;
            return RedirectToAction("CartItem", "ShoppingCart");
        }

        var productName = await shoppingCartService.GetProductNameAsync(cartItemId);

        try
        {
            await shoppingCartService.UpdateQuantityCartAsync(cartItemId, quantity);
            await shoppingCartService.UpdateOrderItemQuantityAsync(cartItemId, quantity);
            return RedirectToAction("CartItem");
        }
        catch (Exception)
        {
            TempData[ErrorMessage] = string.Format(UnexpectedErrorOccurredUpdateQuantity, productName);
            return RedirectToAction("CartItem", "ShoppingCart");
        }
    }

    private IActionResult GeneralError()
    {
        TempData[ErrorMessage] = GeneralErrors;

        return RedirectToAction("Index", "Home");
    }
}
