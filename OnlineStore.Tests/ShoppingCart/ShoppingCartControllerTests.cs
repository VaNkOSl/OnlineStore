namespace OnlineStore.Tests.ShoppingCart;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using OnlineStore.Controllers;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.ShopingCart;
using System.Security.Claims;
using static OnlineStore.Commons.MessagesConstants;
using static OnlineStore.Commons.NotificationMessagesConstants;

public class ShoppingCartControllerTests
{
    private readonly Mock<IShoppingCartService> shopingCartServiceMock;
    private readonly Mock<ISellerService> sellerServiceMock;
    private readonly Mock<IProductService> productServiceMock;

    private readonly ShoppingCartController controller;

    public ShoppingCartControllerTests()
    {
        shopingCartServiceMock = new Mock<IShoppingCartService>();
        productServiceMock = new Mock<IProductService>();
        sellerServiceMock = new Mock<ISellerService>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "testUserId")
        }, "mock"));

        controller = new ShoppingCartController(shopingCartServiceMock.Object, sellerServiceMock.Object,
                                               productServiceMock.Object);

        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        controller.TempData = new TempDataDictionary(
        controller.HttpContext,
        Mock.Of<ITempDataProvider>());
    }

    [Fact]
    public async Task CartItem_Get_ShouldReturnCartItemModel()
    {
        var userId = "testUserId"; 

        var model = new List<CartItemViewModel>
        {
            new CartItemViewModel
            {
                ProductName = "Test product",
                ProductPrice = 100,
                Quantity = 10,
                SelectedColors = new List<string> { "Blue" },
                SelectedSizes = new List<string> { "M" }
            }
        };

        shopingCartServiceMock
            .Setup(s => s.GetAllCartItemAsync(userId))
            .ReturnsAsync(model);

        var result = await controller.CartItem();

        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsAssignableFrom<IEnumerable<CartItemViewModel>>(viewResult.Model);

        Assert.NotNull(returnedModel);
        Assert.Single(returnedModel);
        Assert.Equal("Test product", returnedModel.First().ProductName);
        Assert.Equal(100, returnedModel.First().ProductPrice);
        Assert.Equal(10, returnedModel.First().Quantity);
        Assert.Contains("Blue", returnedModel.First().SelectedColors);
        Assert.Contains("M", returnedModel.First().SelectedSizes);
    }

    [Fact]
    public async Task CartItem_Get_ShouldReturnGeneralErrorView_OnException()
    {
        shopingCartServiceMock
            .Setup(s => s.GetAllCartItemAsync(It.IsAny<string>()))
        .ThrowsAsync(new Exception());

        var result = await controller.CartItem();

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(GeneralErrors, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task AddToCart_Post_ShouldReturnEroor_WhenUserDoesNotSelectColor()
    {
        var productId = Guid.NewGuid().ToString();

        var result = await controller.AddToCart(productId, 5, new List<string>(), new List<string>());

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.NotNull(result);
        Assert.Equal("Details", resultAsRedirect.ActionName);
        Assert.Equal("Product", resultAsRedirect.ControllerName);
        Assert.Equal(productId, resultAsRedirect.RouteValues!["id"]);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(UserDoesNotSelectAColor, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task AddToCart_Post_ShouldReturnEroor_WhenUserDoesNotSelectSize()
    {
        var productId = Guid.NewGuid().ToString();
        var colors = new List<string> { "Blue" };

        var result = await controller.AddToCart(productId,5,colors, new List<string>());

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.NotNull(result);
        Assert.Equal("Details", resultAsRedirect.ActionName);
        Assert.Equal("Product", resultAsRedirect.ControllerName);
        Assert.Equal(productId, resultAsRedirect.RouteValues!["id"]);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(UserDoesNotSelectASize, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task AddToCart_Post_ShouldReturnEroor_WhenUserIsASeller()
    {
        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var productId = Guid.NewGuid().ToString();
        var colors = new List<string> { "Blue" };
        var sizes = new List<string> { "L", "M" };

        var result = await controller.AddToCart(productId, 5, colors, sizes);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.NotNull(result);
        Assert.Equal("Index", resultAsRedirect.ActionName);
        Assert.Equal("Home", resultAsRedirect.ControllerName);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(SellersCannonMakeOrders, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task AddToCart_Post_ShouldRedirectToCartItem_WhenProductAlredyExistsInCartItem()
    {
        var productId = Guid.NewGuid().ToString();
        var colors = new List<string> { "Blue" };
        var sizes = new List<string> { "L", "M" };

        sellerServiceMock
             .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
             .ReturnsAsync(false);

        shopingCartServiceMock
            .Setup(s => s.ProductAlreadyExistInCartItemAsync(productId, It.IsAny<string>()))
            .ReturnsAsync(true);

        productServiceMock
            .Setup(s => s.GetProductNameAsync(productId))
            .ReturnsAsync("Test produc");

        var result = await controller.AddToCart(productId, 5, colors, sizes);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.NotNull(result);
        Assert.Equal("CartItem", resultAsRedirect.ActionName);
        Assert.Equal("ShoppingCart", resultAsRedirect.ControllerName);
        Assert.True(controller.TempData.ContainsKey(WarningMessage));
        Assert.Equal(string.Format(ProductAlreadyAddedToCartItem, "Test produc"), controller.TempData[WarningMessage]);
    }

    [Fact]
    public async Task AddToCart_Post_ShouldSuccessfullyAddItemToCart()
    {
        var productId = Guid.NewGuid().ToString();
        var colors = new List<string> { "Blue" };
        var sizes = new List<string> { "L", "M" };

        sellerServiceMock
             .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
             .ReturnsAsync(false);

        shopingCartServiceMock
            .Setup(s => s.ProductAlreadyExistInCartItemAsync(productId, It.IsAny<string>()))
            .ReturnsAsync(false);

        productServiceMock
            .Setup(s => s.GetProductNameAsync(productId))
            .ReturnsAsync("Test produc");

        shopingCartServiceMock
             .Setup(s => s.AddToCartAsync(It.IsAny<string>(), productId, 5, colors, sizes))
             .Returns(Task.CompletedTask);

        var result = await controller.AddToCart(productId, 5, colors, sizes);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.NotNull(result);
        Assert.Equal("Details", resultAsRedirect.ActionName);
        Assert.Equal("Product", resultAsRedirect.ControllerName);
        Assert.Equal(productId, resultAsRedirect.RouteValues!["id"]);
        Assert.True(controller.TempData.ContainsKey(SuccessMessage));
        Assert.Equal(string.Format(SuccessfullyAddedItemToCart, "Test produc"), controller.TempData[SuccessMessage]);
    }

    [Fact]
    public async Task AddToCart_Post_ShouldRedirectToProductDetails_WhenExceptionIsThrown()
    {
        var productId = Guid.NewGuid().ToString();
        var colors = new List<string> { "Blue" };
        var sizes = new List<string> { "L", "M" };

        sellerServiceMock
             .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
             .ReturnsAsync(false);

        shopingCartServiceMock
            .Setup(s => s.ProductAlreadyExistInCartItemAsync(productId, It.IsAny<string>()))
            .ReturnsAsync(false);

        productServiceMock
            .Setup(s => s.GetProductNameAsync(productId))
            .ReturnsAsync("Test produc");

        shopingCartServiceMock
             .Setup(s => s.AddToCartAsync(It.IsAny<string>(), productId, 5, colors, sizes))
             .ThrowsAsync(new Exception());

        var result = await controller.AddToCart(productId, 5, colors, sizes);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.NotNull(result);
        Assert.Equal("Details", resultAsRedirect.ActionName);
        Assert.Equal("Product", resultAsRedirect.ControllerName);
        Assert.Equal(productId, resultAsRedirect.RouteValues!["id"]);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(string.Format(UnexpectedErrorOccurredAddToCartItem, "Test produc"), controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task RemoveFromCart_Post_ShouldRedirectToHomeIndex_WhenUserIsASeller()
    {
        var cartItemId = Guid.NewGuid().ToString();

        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await controller.RemoveFromCart(cartItemId);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.NotNull(result);
        Assert.Equal("Index", resultAsRedirect.ActionName);
        Assert.Equal("Home", resultAsRedirect.ControllerName);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(SellersCannonMakeOrders, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task RemoveFromCart_Post_ShouldRemoveCartItemSuccessfully_WhenUserIsNotASeller()
    {
        var cartItemId = Guid.NewGuid().ToString();

        sellerServiceMock
         .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
         .ReturnsAsync(false);

        shopingCartServiceMock
               .Setup(s => s.GetProductNameAsync(cartItemId))
               .ReturnsAsync("Test produc");

        shopingCartServiceMock
            .Setup(s => s.RemoveFromCartAsync(cartItemId))
            .Returns(Task.CompletedTask);

        var result = await controller.RemoveFromCart(cartItemId);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.NotNull(result);
        Assert.Equal("CartItem", resultAsRedirect.ActionName);
        Assert.True(controller.TempData.ContainsKey(WarningMessage));
        Assert.Equal(string.Format(SuccessfullyDeleteProductFromCart, "Test produc"), controller.TempData[WarningMessage]);
    }

    [Fact]
    public async Task RemoveFromCart_Post_ShouldRedirectToCartItemShoppingCart_WhenExceptionIsThrown()
    {
        var cartItemId = Guid.NewGuid().ToString();

        sellerServiceMock
         .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
         .ReturnsAsync(false);

        shopingCartServiceMock
               .Setup(s => s.GetProductNameAsync(cartItemId))
               .ReturnsAsync("Test produc");

        shopingCartServiceMock
            .Setup(s => s.RemoveFromCartAsync(cartItemId))
            .ThrowsAsync(new Exception());

        var result = await controller.RemoveFromCart(cartItemId);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.NotNull(result);
        Assert.Equal("CartItem", resultAsRedirect.ActionName);
        Assert.Equal("ShoppingCart", resultAsRedirect.ControllerName);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(string.Format(UnexpectedErrorOccurredRemoveFromCart, "Test produc"), controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task UpdateColors_Post_ShouldRedirectToHomeIndex_WhenUserIsASeller()
    {
        var cartItemId = Guid.NewGuid().ToString();
        var colors = new List<string> { "Colors" };

        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await controller.UpdateColors(cartItemId, colors);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Index", resultAsRedirect.ActionName);
        Assert.Equal("Home", resultAsRedirect.ControllerName);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(SellersCannonMakeOrders, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task UpdateColors_Post_ShouldRedirectToCartItemShoppingCart_WhenUserDoesNotSelectAColor()
    {
        var cartItemId = Guid.NewGuid().ToString();
        var colors = new List<string>();

        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await controller.UpdateColors(cartItemId, colors);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("CartItem", resultAsRedirect.ActionName);
        Assert.Equal("ShoppingCart", resultAsRedirect.ControllerName);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(UserDoesNotSelectAColor, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task UpdateColors_Post_ShouldUpdateColorsFromCartItemAndOrderItemSuccessfully_WhenUserIsSelectAColor()
    {
        var cartItemId = Guid.NewGuid().ToString();
        var colors = new List<string> { "Blue" };

        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        shopingCartServiceMock
            .Setup(s => s.GetProductNameAsync(cartItemId))
            .ReturnsAsync("Test Product");

        shopingCartServiceMock
            .Setup(s => s.UpdateColorsAsync(cartItemId,colors))
            .Returns(Task.CompletedTask);

        shopingCartServiceMock
          .Setup(s => s.UpdateOrderItemsColorsAsync(cartItemId, colors))
          .Returns(Task.CompletedTask);

        var result = await controller.UpdateColors(cartItemId, colors);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.NotNull(result);
        Assert.Equal("CartItem", resultAsRedirect.ActionName);
    }

    [Fact]
    public async Task UpdateColors_Post_ShouldRedirectToCartItemShoppingCart_WhenExceptionItThrows()
    {
        var cartItemId = Guid.NewGuid().ToString();
        var colors = new List<string> { "Blue" };

        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        shopingCartServiceMock
            .Setup(s => s.GetProductNameAsync(cartItemId))
            .ReturnsAsync("Test Product");

        shopingCartServiceMock
            .Setup(s => s.UpdateColorsAsync(cartItemId, colors))
            .ThrowsAsync(new Exception());

        shopingCartServiceMock
          .Setup(s => s.UpdateOrderItemsColorsAsync(cartItemId, colors))
          .ThrowsAsync(new Exception());

        var result = await controller.UpdateColors(cartItemId, colors);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.NotNull(result);
        Assert.Equal("CartItem", resultAsRedirect.ActionName);
        Assert.Equal("ShoppingCart", resultAsRedirect.ControllerName);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(string.Format(UnexpectedErrorOccurredUpdateColor, "Test Product"), controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task UpdateSizes_Post_ShouldRedirectToHomeIndex_WhenUserIsASeller()
    {
        var cartItemId = Guid.NewGuid().ToString();
        var sizes = new List<string>() { "M"};

        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await controller.UpdateSizes(cartItemId, sizes);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Index", resultAsRedirect.ActionName);
        Assert.Equal("Home", resultAsRedirect.ControllerName);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(SellersCannonMakeOrders, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task UpdateSizes_Post_ShouldRedirectToCartItemShoppingCart_WhenUserDoesNotSelectASize()
    {
        var cartItemId = Guid.NewGuid().ToString();
        var sizes = new List<string>();

        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await controller.UpdateSizes(cartItemId, sizes);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);

        Assert.NotNull(result);
        Assert.Equal("CartItem", resultAsRedirect.ActionName);
        Assert.Equal("ShoppingCart", resultAsRedirect.ControllerName);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(UserDoesNotSelectASize, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task UpdateSizes_Post_ShouldRedirectToCartItem_WhenUpdateIsSuccessful()
    {
        var cartItemId = Guid.NewGuid().ToString();
        var sizes = new List<string> { "M", "L" };
        var productName = "TestProduct";

        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        shopingCartServiceMock
            .Setup(s => s.GetProductNameAsync(It.IsAny<string>()))
            .ReturnsAsync(productName);

        shopingCartServiceMock
            .Setup(s => s.UpdateSizesAsync(cartItemId, sizes))
            .Returns(Task.CompletedTask);

        shopingCartServiceMock
            .Setup(s => s.UpdateOrderItemsSizesAsync(cartItemId, sizes))
            .Returns(Task.CompletedTask);

        var result = await controller.UpdateSizes(cartItemId, sizes);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.NotNull(result);
        Assert.Equal("CartItem", redirectResult.ActionName);
        Assert.False(controller.TempData.ContainsKey(ErrorMessage));
    }

    [Fact]
    public async Task UpdateSizes_Post_ShouldRedirectToCartItem_WithErrorMessage_WhenExceptionIsThrown()
    {
        var cartItemId = Guid.NewGuid().ToString();
        var sizes = new List<string> { "M", "L" };
        var productName = "TestProduct";

        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        shopingCartServiceMock
            .Setup(s => s.GetProductNameAsync(It.IsAny<string>()))
            .ReturnsAsync(productName);

        shopingCartServiceMock
            .Setup(s => s.UpdateSizesAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
            .ThrowsAsync(new Exception());

        shopingCartServiceMock
            .Setup(s => s.UpdateOrderItemsSizesAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
            .ThrowsAsync(new Exception());

        var result = await controller.UpdateSizes(cartItemId, sizes);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("CartItem", redirectResult.ActionName);
        Assert.Equal("ShoppingCart", redirectResult.ControllerName);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(string.Format(UnexpectedErrorOccurredUpdateSize, productName), controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task UpdateQuantity_Post_ShouldRedirectToCartItemShoppingCart_WhenUserDoesNotSelectASize()
    {
        var cartItemId = Guid.NewGuid().ToString();
        int quantity = 4;

        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await controller.UpdateQuantity(cartItemId, quantity);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.NotNull(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(SellersCannonMakeOrders, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task UpdateQuantity_Post_ShouldRedirectToCartItemShoppingCart_WhenUserDoesNotSelectAQuantity()
    {
        var cartItemId = Guid.NewGuid().ToString();
        int quantity = 0;

        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await controller.UpdateQuantity(cartItemId, quantity);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.NotNull(result);
        Assert.Equal("CartItem", redirectResult.ActionName);
        Assert.Equal("ShoppingCart", redirectResult.ControllerName);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(QuantityMustBePositiveNumber, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task UpdateQuantity_Post_ShouldRedirectToCartItem_WhenUpdateIsSuccessful()
    {
        var cartItemId = Guid.NewGuid().ToString();
        int quantity = 5;

        sellerServiceMock
           .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
           .ReturnsAsync(false);

        shopingCartServiceMock
            .Setup(s => s.GetProductNameAsync(cartItemId))
            .ReturnsAsync("Test product");

        shopingCartServiceMock
            .Setup(s => s.UpdateQuantityCartAsync(cartItemId, quantity))
            .Returns(Task.CompletedTask);

        shopingCartServiceMock
         .Setup(s => s.UpdateOrderItemQuantityAsync(cartItemId, quantity))
         .Returns(Task.CompletedTask);

        var result = await controller.UpdateQuantity(cartItemId, quantity);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.NotNull(result);
        Assert.Equal("CartItem", redirectResult.ActionName);
        Assert.False(controller.TempData.ContainsKey(ErrorMessage));
    }

    [Fact]
    public async Task UpdateQuantity_ShouldRedirectToCartItem_WhenExceptionIsThrows()
    {
        var cartItemId = Guid.NewGuid().ToString();
        int quantity = 5;

        sellerServiceMock
           .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
           .ReturnsAsync(false);

        shopingCartServiceMock
            .Setup(s => s.GetProductNameAsync(cartItemId))
            .ReturnsAsync("Test product");

        shopingCartServiceMock
            .Setup(s => s.UpdateQuantityCartAsync(cartItemId, quantity))
            .ThrowsAsync(new Exception());

        shopingCartServiceMock
             .Setup(s => s.UpdateOrderItemQuantityAsync(cartItemId, quantity))
             .ThrowsAsync(new Exception());

        var result = await controller.UpdateQuantity(cartItemId, quantity);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.NotNull(result);
        Assert.Equal("CartItem", redirectResult.ActionName);
        Assert.Equal("ShoppingCart", redirectResult.ControllerName);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(string.Format(UnexpectedErrorOccurredUpdateQuantity, "Test product"), controller.TempData[ErrorMessage]);
    }
}
