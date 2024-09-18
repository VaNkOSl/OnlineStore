namespace OnlineStore.Tests.ShoppingCart;

using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Data.Data.Common;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Services.Data;
using OnlineStore.Services.Data.Contacts;
using static OnlineStore.Commons.EntityValidationConstraints;
using static OnlineStore.Tests.DataBaseSeeder;
public class ShoppingCartServiceTests
{
    private DbContextOptions<OnlineStoreDbContext> dbOptions;
    private OnlineStoreDbContext dbContext;

    private readonly IShoppingCartService shoppingCartService;

    public ShoppingCartServiceTests()
    {
        dbOptions = new DbContextOptionsBuilder<OnlineStoreDbContext>()
            .UseInMemoryDatabase("OnlineStoreInMemory" + Guid.NewGuid().ToString())
            .Options;

        dbContext = new OnlineStoreDbContext(dbOptions);
        dbContext.Database.EnsureCreated();
        SeedDataBase(dbContext);

        IRepository repository = new Repository(dbContext);

        shoppingCartService = new ShoppingCartService(repository);
    }

    [Fact]
    public async Task AddToCartAsync_ShouldAddItemToCartSuccessfully()
    {
        var userId = User!.Id.ToString();
        var productId = DataBaseSeeder.Product!.Id.ToString();
        int quantity = 2;
        List<string> selectedColors = new List<string> { "Blue" };
        List<string> selectedSizes = new List<string> { "L", "M" };

        await shoppingCartService.AddToCartAsync(userId, productId, quantity, selectedColors, selectedSizes);

        var cartItem = await dbContext.CartItems
            .FirstOrDefaultAsync(c => c.UserId.ToString() == userId && c.ProductId.ToString() == productId);

        var order = await dbContext.Orders
            .FirstOrDefaultAsync(o => o.UserId.ToString() == userId && o.OrderStatus == OrderStatus.Cart);

        var orderItem = await dbContext.OrderItems
            .FirstOrDefaultAsync(or => or.ProductId.ToString() == productId && or.OrderId == order!.Id);

        Assert.NotNull(cartItem);
        Assert.Equal(quantity, cartItem.Quantity);
        Assert.Equal(selectedColors, cartItem.SelectedColor);

        Assert.NotNull(orderItem);
        Assert.Equal(order!.Id, orderItem.OrderId);
        Assert.Equal(selectedColors, orderItem.SelectedColor);
        Assert.Equal(selectedSizes, orderItem.SelectedSizes);
        Assert.Equal(productId, orderItem.ProductId.ToString());

        Assert.NotNull(order);
        Assert.Equal(OrderStatus.Cart, order.OrderStatus);
    }

    [Fact]
    public async Task ClearCartItemsAsync_ShouldRemoveAllItemsFromCart()
    {
        var userId = Guid.NewGuid();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var cartItem1 = new CartItem
        {
            UserId = userId,
            ProductId = productId1,
            Quantity = 1,
            SelectedColor = new List<string> { "Blue" },
            SelectedSizes = new List<string> { "M" }
        };

        var cartItem2 = new CartItem
        {
            UserId = userId,
            ProductId = productId2,
            Quantity = 2,
            SelectedColor = new List<string> { "Red" },
            SelectedSizes = new List<string> { "L" }
        };

        await dbContext.CartItems.AddAsync(cartItem1);
        await dbContext.CartItems.AddAsync(cartItem2);
        await dbContext.SaveChangesAsync();

        await shoppingCartService.ClearCartItemsAsync();

        var cartItemsAfterClear = await dbContext.CartItems.ToListAsync();
        Assert.Empty(cartItemsAfterClear); 
    }

    [Fact]
    public async Task GetAllCartItemAsync_ShouldReturnCountOfProductInCartItem_WhenUserHasProducts()
    {
        var userId = User!.Id.ToString();

        var result = await shoppingCartService.GetAllCartItemAsync(userId);

        var cartItem = await dbContext.CartItems
            .FirstOrDefaultAsync(c => c.UserId.ToString() == userId);

        Assert.NotEmpty(result);
        Assert.Single(result);
        Assert.Equal("First Product", cartItem!.Product.Name);
        Assert.Equal(2, cartItem.Quantity);
    }

    [Fact]
    public async Task GetProductNameAsync_ShouldReturnCorrectProductName()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();

        var result = await shoppingCartService.GetProductNameAsync(cartItemId);

        Assert.Contains("First Product", result);
    }

    [Fact]
    public async Task GetProductNameAsync_ShouldReturnUnknownProductName_WhenCartItemIdDoesNotExists()
    {
        var notExistingCartItemId = Guid.NewGuid().ToString();

        var result = await shoppingCartService.GetProductNameAsync(notExistingCartItemId);

        Assert.Contains("Unknown product name", result);
    }

    [Fact]
    public async Task ProductAlreadyExistInCartItemAsync_ShouldReturnTrue_WhenProductExists()
    {
        var userId = User!.Id.ToString();
        var productId = DataBaseSeeder.Product!.Id.ToString();

        bool result = await shoppingCartService.ProductAlreadyExistInCartItemAsync(productId, userId);

        Assert.True(result);
    }

    [Fact]
    public async Task ProductAlreadyExistInCartItemAsync_ShouldReturnFalse_WhenUserDoesNotExists()
    {
        var notExistigUserId = Guid.NewGuid().ToString();
        var productId = DataBaseSeeder.Product!.Id.ToString();

        bool result = await shoppingCartService.ProductAlreadyExistInCartItemAsync(productId, notExistigUserId);

        Assert.False(result);
    }

    [Fact]
    public async Task ProductAlreadyExistInCartItemAsync_ShouldReturnFalse_WhenProductDoesNotExists()
    {
        var userId = User!.Id.ToString();
        var notExistingProductId = Guid.NewGuid().ToString();

        bool result = await shoppingCartService.ProductAlreadyExistInCartItemAsync(notExistingProductId, userId);

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveFromCartAsync_ShouldRemoveItemsFromCartItemsAndOrderItems_WhenCartItemIdExists()
    {
        Assert.Equal(1, dbContext.CartItems.Count());
        Assert.Equal(1, dbContext.OrderItems.Count());
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();

        await shoppingCartService.RemoveFromCartAsync(cartItemId);

        Assert.Equal(0, dbContext.CartItems.Count());
        Assert.Equal(0, dbContext.OrderItems.Count());
    }

    [Fact]  
    public async Task RemoveFromCartAsync_ShouldThrowException_WhenCartItemIdDoesNotExists()
    {
        var nonExistentCartItemId = Guid.NewGuid().ToString();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            shoppingCartService.RemoveFromCartAsync(nonExistentCartItemId)
        );

        Assert.Equal(1, dbContext.CartItems.Count());
        Assert.Equal(1, dbContext.OrderItems.Count());
    }

    [Fact]
    public async Task UpdateColorsAsync_ShouldUpdateColorSuccessfully_WhenCartItemIdExists()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();

        var originalCartItem = await dbContext
            .CartItems
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(originalCartItem);
        Assert.Contains("Blue", originalCartItem!.SelectedColor);  

        List<string> updatedColor = new List<string> { "Green" };

        await shoppingCartService.UpdateColorsAsync(cartItemId, updatedColor);

        var updatedCartItem = await dbContext.CartItems
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(updatedCartItem);

        Assert.Contains("Green", updatedCartItem!.SelectedColor);
        Assert.DoesNotContain("Blue", updatedCartItem.SelectedColor); 
    }

    [Fact]
    public async Task UpdateColorsAsync_ShouldDoNothing_WhenCartItemIdDoesNotExist()
    {
        var nonExistentCartItemId = Guid.NewGuid().ToString();
        var colors = new List<string> { "Red" };

        await shoppingCartService.UpdateColorsAsync(nonExistentCartItemId, colors);

        var cartItems = await dbContext.CartItems
            .AsNoTracking()
            .ToListAsync();

        Assert.DoesNotContain(cartItems, ci => ci.Id.ToString() == nonExistentCartItemId);
    }

    [Fact]
    public async Task UpdateColorsAsync_ShouldSetColorsToEmptyList_WhenColorsListIsEmpty()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();
        var colors = new List<string>(); 

        await shoppingCartService.UpdateColorsAsync(cartItemId, colors);

        var updatedCartItem = await dbContext.CartItems
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(updatedCartItem);
        Assert.Empty(updatedCartItem!.SelectedColor); 
    }

    [Fact]
    public async Task UpdateColorsAsync_ShouldIgnoreNullColors_WhenColorsListContainsNulls()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();
        var colorsWithNulls = new List<string> { "Green", null!, "Blue" }; 

        await shoppingCartService.UpdateColorsAsync(cartItemId, colorsWithNulls);

        var updatedCartItem = await dbContext
            .CartItems
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(updatedCartItem);
        Assert.Contains("Green", updatedCartItem!.SelectedColor);
        Assert.Contains("Blue", updatedCartItem.SelectedColor);
        Assert.DoesNotContain(null, updatedCartItem.SelectedColor); 
    }

    [Fact]
    public async Task UpdateColorsAsync_ShouldRemoveDuplicateColors_WhenColorsListContainsDuplicates()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();
        var colorsWithDuplicates = new List<string> { "Green", "Blue", "Green", "Red", "Blue" };

        await shoppingCartService.UpdateColorsAsync(cartItemId, colorsWithDuplicates);

        var updatedCartItem = await dbContext.CartItems
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(updatedCartItem);
        Assert.Contains("Green", updatedCartItem!.SelectedColor);
        Assert.Contains("Blue", updatedCartItem.SelectedColor);
        Assert.Contains("Red", updatedCartItem.SelectedColor);
        Assert.Equal(3, updatedCartItem.SelectedColor.Count); 
    }

    [Fact]
    public async Task UpdateQuantityCartAsync_ShouldUpdateQuantitySuccessfully_WhenCartItemIdExists()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();

        var originalCartItem = await dbContext
             .CartItems
             .AsNoTracking()
             .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(originalCartItem);
        Assert.Equal(2, originalCartItem.Quantity);

        int updateQuantity = 3;

        await shoppingCartService.UpdateQuantityCartAsync(cartItemId, updateQuantity);

        var updatedCartItem = await dbContext
              .CartItems
              .AsNoTracking()
              .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(updatedCartItem);
        Assert.Equal(3, updatedCartItem.Quantity);
        Assert.NotEqual(originalCartItem.Quantity, updatedCartItem.Quantity);
    }

    [Fact]
    public async Task UpdateQuantityCartAsync_ShouldThrowArgumentException_WhenQuantityIsZeroOrNegative()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();

        await Assert.ThrowsAsync<ArgumentException>(() => shoppingCartService.UpdateQuantityCartAsync(cartItemId, 0));

        await Assert.ThrowsAsync<ArgumentException>(() => shoppingCartService.UpdateQuantityCartAsync(cartItemId, -5));
    }

    [Fact]
    public async Task UpdateOrderItemsColorsAsync_ShouldUpdateSuccessfully_WhenCartItemIdExists()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();

        var cartItem = await dbContext
              .CartItems
             .AsNoTracking()
             .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(cartItem);
        Assert.Contains("Blue", cartItem.SelectedColor);

        var order = await dbContext
              .Orders
             .AsNoTracking()
             .FirstOrDefaultAsync(o => o.UserId == cartItem!.UserId 
                                 && o.OrderStatus == OrderStatus.Cart);

        Assert.NotNull(order);
        Assert.Equal(cartItem.UserId, order.UserId);

        var orderItem = await dbContext
            .OrderItems
            .AsNoTracking()
            .FirstOrDefaultAsync(oi => oi.OrderId == order.Id && oi.ProductId == cartItem.ProductId);

        Assert.NotNull(orderItem);
        Assert.Equal(orderItem.OrderId, order.Id);
        Assert.Equal(orderItem.ProductId,cartItem.ProductId);
        Assert.Contains("Blue", orderItem.SelectedColor);

        var updatedColor = new List<string> { "Green" };

        await shoppingCartService.UpdateOrderItemsColorsAsync(cartItemId, updatedColor);

        var updatedOrderItem = await dbContext
            .OrderItems
            .AsNoTracking()
            .FirstOrDefaultAsync(oi => oi.OrderId == order.Id && oi.ProductId == cartItem.ProductId);

        Assert.Contains("Green", updatedOrderItem!.SelectedColor);
        Assert.DoesNotContain("Blue", updatedOrderItem.SelectedColor);
    }

    [Fact]
    public async Task UpdateOrderItemsColorsAsync_ShouldThrowArgumentException_WhenCartItemIdDoesNotExist()
    {
        var notExistingCartItemId = Guid.NewGuid().ToString(); 
        var colors = new List<string> { "Red", "Green" };

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            shoppingCartService.UpdateOrderItemsColorsAsync(notExistingCartItemId, colors));

        Assert.Equal("Cart item not found!", exception.Message);
    }

    [Fact]
    public async Task UpdateOrderItemsColorsAsync_ShouldSetColorToEmptyList_WhenColorListIsEmpty()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();
        var colors = new List<string>();

        var cartItem = await dbContext
          .CartItems
         .AsNoTracking()
         .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        var order = await dbContext
            .Orders
           .AsNoTracking()
           .FirstOrDefaultAsync(o => o.UserId == cartItem!.UserId
                               && o.OrderStatus == OrderStatus.Cart);

        await shoppingCartService.UpdateOrderItemsColorsAsync(cartItemId, colors);

        var updatedOrderItem = await dbContext
             .OrderItems
             .AsNoTracking()
             .FirstOrDefaultAsync(oi => oi.OrderId == order!.Id && oi.ProductId == cartItem!.ProductId);

        Assert.NotNull(updatedOrderItem);
        Assert.Empty(updatedOrderItem!.SelectedColor);
    }

    [Fact]
    public async Task UpdateOrderItemsColorsAsync_ShouldIgnoreNull_WhenColorsListContainsNulls()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();
        var colors = new List<string> { "Green", null!, "Black" };

        var cartItem = await dbContext
          .CartItems
         .AsNoTracking()
         .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        var order = await dbContext
            .Orders
           .AsNoTracking()
           .FirstOrDefaultAsync(o => o.UserId == cartItem!.UserId
                               && o.OrderStatus == OrderStatus.Cart);

        await shoppingCartService.UpdateOrderItemsColorsAsync(cartItemId, colors);

        var updatedOrderItem = await dbContext
             .OrderItems
             .AsNoTracking()
             .FirstOrDefaultAsync(oi => oi.OrderId == order!.Id && oi.ProductId == cartItem!.ProductId);

        Assert.NotNull(updatedOrderItem);
        Assert.Contains("Green", updatedOrderItem.SelectedColor);
        Assert.Contains("Black", updatedOrderItem.SelectedColor);
        Assert.DoesNotContain(null, updatedOrderItem.SelectedColor);
    }

    [Fact]
    public async Task UpdateOrderItemsColorsAsync_ShouldRemoveDuplicateColors_WhenColorsListContainsDuplicates()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();
        var colors = new List<string> { "Green", "Green", "Black", "Black" };

        var cartItem = await dbContext
          .CartItems
         .AsNoTracking()
         .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        var order = await dbContext
            .Orders
           .AsNoTracking()
           .FirstOrDefaultAsync(o => o.UserId == cartItem!.UserId
                               && o.OrderStatus == OrderStatus.Cart);

        await shoppingCartService.UpdateOrderItemsColorsAsync(cartItemId, colors);

        var updatedOrderItem = await dbContext
             .OrderItems
             .AsNoTracking()
             .FirstOrDefaultAsync(oi => oi.OrderId == order!.Id && oi.ProductId == cartItem!.ProductId);

        Assert.NotNull(updatedOrderItem);
        Assert.Contains("Green", updatedOrderItem.SelectedColor);
        Assert.Contains("Black", updatedOrderItem.SelectedColor);
        Assert.Equal(2, updatedOrderItem.SelectedColor.Count);
    }

    [Fact]
    public async Task UpdateOrderItemsSizesAsync_ShouldUpdateSuccessfully_WhenCartItemIdExists()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();

        var cartItem = await dbContext
              .CartItems
             .AsNoTracking()
             .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(cartItem);
        Assert.Contains("M", cartItem.SelectedSizes);

        var order = await dbContext
              .Orders
              .AsNoTracking()
              .FirstOrDefaultAsync(o => o.UserId == cartItem!.UserId
                                 && o.OrderStatus == OrderStatus.Cart);

        Assert.NotNull(order);
        Assert.Equal(cartItem.UserId, order.UserId);

        var orderItem = await dbContext
                .OrderItems
                .AsNoTracking()
                .FirstOrDefaultAsync(oi => oi.OrderId == order.Id && oi.ProductId == cartItem.ProductId);

        Assert.NotNull(orderItem);
        Assert.Equal(orderItem.OrderId, order.Id);
        Assert.Equal(orderItem.ProductId, cartItem.ProductId);
        Assert.Contains("M", orderItem.SelectedSizes);

        var updateSize = new List<string> { "XL" };

        await shoppingCartService.UpdateOrderItemsSizesAsync(cartItemId, updateSize);

        var updatedOrderItem = await dbContext
            .OrderItems
            .AsNoTracking()
            .FirstOrDefaultAsync(oi => oi.OrderId == order.Id 
                              && oi.ProductId == cartItem.ProductId);

        Assert.Contains("XL", updatedOrderItem!.SelectedSizes);
        Assert.DoesNotContain("M", updatedOrderItem!.SelectedSizes);
    }

    [Fact]
    public async Task UpdateOrderItemsSizesAsync_ShouldThrowArgumentException_WhenCartItemIdDoesNotExist()
    {
        var cartItemId = Guid.NewGuid().ToString();

        var sizes = new List<string> { "M", "XXL" };

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
             shoppingCartService.UpdateOrderItemsSizesAsync(cartItemId,sizes));

        Assert.Equal("Cart item not found!",exception.Message);
    }

    [Fact]
    public async Task UpdateOrderItemsSizesAsync_ShouldSetSizesToEmptyList_WhenSizesListIsEmpty()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();

        var cartItem = await dbContext
              .CartItems
             .AsNoTracking()
             .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(cartItem);
        Assert.Contains("M", cartItem.SelectedSizes);

        var order = await dbContext
              .Orders
              .AsNoTracking()
              .FirstOrDefaultAsync(o => o.UserId == cartItem!.UserId
                                 && o.OrderStatus == OrderStatus.Cart);

        Assert.NotNull(order);
        Assert.Equal(cartItem.UserId, order.UserId);

        var orderItem = await dbContext
                .OrderItems
                .AsNoTracking()
                .FirstOrDefaultAsync(oi => oi.OrderId == order.Id && oi.ProductId == cartItem.ProductId);

        Assert.NotNull(orderItem);
        Assert.Equal(orderItem.OrderId, order.Id);
        Assert.Equal(orderItem.ProductId, cartItem.ProductId);
        Assert.Contains("M", orderItem.SelectedSizes);

        var updateSize = new List<string>();

        await shoppingCartService.UpdateOrderItemsSizesAsync(cartItemId, updateSize);

        var updatedOrderItem = await dbContext
                 .OrderItems
                 .AsNoTracking()
                 .FirstOrDefaultAsync(oi => oi.OrderId == order.Id
                                   && oi.ProductId == cartItem.ProductId);

        Assert.NotNull(updatedOrderItem);
        Assert.Empty(updatedOrderItem.SelectedSizes);
    }

    [Fact]
    public async Task UpdateOrderItemsSizesAsync_ShouldIgnoreNull_WhenSizesListContainsNull()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();

        var cartItem = await dbContext
              .CartItems
             .AsNoTracking()
             .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(cartItem);
        Assert.Contains("M", cartItem.SelectedSizes);

        var order = await dbContext
              .Orders
              .AsNoTracking()
              .FirstOrDefaultAsync(o => o.UserId == cartItem!.UserId
                                 && o.OrderStatus == OrderStatus.Cart);

        Assert.NotNull(order);
        Assert.Equal(cartItem.UserId, order.UserId);

        var orderItem = await dbContext
                .OrderItems
                .AsNoTracking()
                .FirstOrDefaultAsync(oi => oi.OrderId == order.Id && oi.ProductId == cartItem.ProductId);

        Assert.NotNull(orderItem);
        Assert.Equal(orderItem.OrderId, order.Id);
        Assert.Equal(orderItem.ProductId, cartItem.ProductId);
        Assert.Contains("M", orderItem.SelectedSizes);

        var updateSize = new List<string> { "M", "L", null! };

        await shoppingCartService.UpdateOrderItemsSizesAsync(cartItemId, updateSize);

        var updatedOrderItem = await dbContext
                 .OrderItems
                 .AsNoTracking()
                 .FirstOrDefaultAsync(oi => oi.OrderId == order.Id
                                   && oi.ProductId == cartItem.ProductId);

        Assert.NotNull(updatedOrderItem);
        Assert.Contains("M", updatedOrderItem.SelectedSizes);
        Assert.Contains("L", updatedOrderItem.SelectedSizes);
        Assert.DoesNotContain(null, updatedOrderItem.SelectedSizes);
    }

    [Fact]
    public async Task UpdateOrderItemsSizesAsync_ShouldRemoveeDuplicateSizes_WhenSizesListContainsDuplicate()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();

        var cartItem = await dbContext
              .CartItems
             .AsNoTracking()
             .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(cartItem);
        Assert.Contains("M", cartItem.SelectedSizes);

        var order = await dbContext
              .Orders
              .AsNoTracking()
              .FirstOrDefaultAsync(o => o.UserId == cartItem!.UserId
                                 && o.OrderStatus == OrderStatus.Cart);

        Assert.NotNull(order);
        Assert.Equal(cartItem.UserId, order.UserId);

        var orderItem = await dbContext
                .OrderItems
                .AsNoTracking()
                .FirstOrDefaultAsync(oi => oi.OrderId == order.Id && oi.ProductId == cartItem.ProductId);

        Assert.NotNull(orderItem);
        Assert.Equal(orderItem.OrderId, order.Id);
        Assert.Equal(orderItem.ProductId, cartItem.ProductId);
        Assert.Contains("M", orderItem.SelectedSizes);

        var updateSize = new List<string> { "M", "L", "M","L" };

        await shoppingCartService.UpdateOrderItemsSizesAsync(cartItemId, updateSize);

        var updatedOrderItem = await dbContext
                 .OrderItems
                 .AsNoTracking()
                 .FirstOrDefaultAsync(oi => oi.OrderId == order.Id
                                   && oi.ProductId == cartItem.ProductId);

        Assert.NotNull(updatedOrderItem);
        Assert.Contains("M", updatedOrderItem.SelectedSizes);
        Assert.Contains("L", updatedOrderItem.SelectedSizes);
        Assert.Equal(2, updatedOrderItem.SelectedSizes.Count);
    }

    [Fact]
    public async Task UpdateOrderItemQuantityAsync_ShouldUpdateQuantitySuccessfully_WhenCartItemIdExists()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();

        var cartItem = await dbContext
              .CartItems
             .AsNoTracking()
             .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(cartItem);

        var order = await dbContext
              .Orders
              .AsNoTracking()
              .FirstOrDefaultAsync(o => o.UserId == cartItem!.UserId
                                 && o.OrderStatus == OrderStatus.Cart);

        Assert.NotNull(order);
        Assert.Equal(cartItem.UserId, order.UserId);

        var orderItem = await dbContext
                .OrderItems
                .AsNoTracking()
                .FirstOrDefaultAsync(oi => oi.OrderId == order.Id && oi.ProductId == cartItem.ProductId);

        Assert.NotNull(orderItem);
        Assert.Equal(orderItem.OrderId, order.Id);
        Assert.Equal(orderItem.ProductId, cartItem.ProductId);
        Assert.Equal(orderItem.Quantity, cartItem.Quantity);

        int updateQuantity =  5;

        await shoppingCartService.UpdateOrderItemQuantityAsync(cartItemId, updateQuantity);

        var updatedOrderItem = await dbContext
         .OrderItems
         .AsNoTracking()
         .FirstOrDefaultAsync(oi => oi.OrderId == order.Id
                           && oi.ProductId == cartItem.ProductId);

        Assert.NotNull(updatedOrderItem);
        Assert.Equal(5, updatedOrderItem.Quantity);
    }

    [Fact]
    public async Task UpdateOrderItemQuantityAsync_ShouldThrowArgumentException_WhenCartItemIdDoesNotExist()
    {
        var cartItemId = Guid.NewGuid().ToString();

        int updateQuantity = 5;

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
             shoppingCartService.UpdateOrderItemQuantityAsync(cartItemId, updateQuantity));

        Assert.Equal("Cart item not found!", exception.Message);
    }

    [Fact]
    public async Task UpdateOrderItemQuantityAsync_ShouldThrowArgumentException_WhenQuantityIsLessOrEqualToZero()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();

        await Assert.ThrowsAsync<ArgumentException>(() => shoppingCartService.UpdateOrderItemQuantityAsync(cartItemId, 0));
        await Assert.ThrowsAsync<ArgumentException>(() => shoppingCartService.UpdateOrderItemQuantityAsync(cartItemId, -5));
    }

    [Fact]
    public async Task UpdateSizesAsync_ShouldUpdateSizeSuccessfully_WhenCartItemIdExists()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();
        var updateSize = new List<string> { "XXL" };

        var originalCartItem = await dbContext
           .CartItems
           .AsNoTracking()
           .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(originalCartItem);
        Assert.Contains("M", originalCartItem.SelectedSizes);

        await shoppingCartService.UpdateSizesAsync(cartItemId, updateSize);

        var updatedCartItem = await dbContext.CartItems
           .AsNoTracking()
           .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(updatedCartItem);
        Assert.Contains("XXL", updatedCartItem.SelectedSizes);
        Assert.DoesNotContain("M", updatedCartItem.SelectedSizes);
    }

    [Fact]
    public async Task UpdateSizesAsync_ShouldDoNothing_WhenCartItemIdDoesNotExist()
    {
        var nonExistentCartItemId = Guid.NewGuid().ToString();
        var sizes = new List<string> { "XXXL" };

        await shoppingCartService.UpdateSizesAsync(nonExistentCartItemId, sizes);

        var cartItems = await dbContext
            .CartItems
            .AsNoTracking()
            .ToListAsync();

        Assert.DoesNotContain(cartItems, ci => ci.Id.ToString() == nonExistentCartItemId);
    }

    [Fact]
    public async Task UpdateSizesAsync_ShouldSetSizesToEmptyList_WhenSizesListIsEmpty()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();
        var sizes = new List<string>();

        await shoppingCartService.UpdateSizesAsync(cartItemId, sizes);

        var updatedCartItem = await dbContext.CartItems
           .AsNoTracking()
           .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(updatedCartItem);
        Assert.Empty(updatedCartItem!.SelectedSizes);
    }

    [Fact]
    public async Task UpdateSizesAsync_ShouldIgnoreNullSizes_WhenSizesListContainsNull()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();
        var sizes = new List<string> { "M", "L", null! };

        await shoppingCartService.UpdateSizesAsync(cartItemId, sizes);

        var updatedCartItem = await dbContext.CartItems
           .AsNoTracking()
           .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(updatedCartItem);
        Assert.Contains("M", updatedCartItem.SelectedSizes);
        Assert.Contains("L", updatedCartItem.SelectedSizes);
        Assert.DoesNotContain(null, updatedCartItem.SelectedSizes);
    }

    [Fact]
    public async Task UpdateSizesAsync_ShouldRemoveDuplicateSizes_WhenSizesListContainsDuplicates()
    {
        var cartItemId = DataBaseSeeder.CartItem!.Id.ToString();
        var sizes = new List<string> { "M", "L", "M", "L" };

        await shoppingCartService.UpdateSizesAsync(cartItemId, sizes);

        var updatedCartItem = await dbContext.CartItems
           .AsNoTracking()
           .FirstOrDefaultAsync(c => c.Id.ToString() == cartItemId);

        Assert.NotNull(updatedCartItem);
        Assert.Contains("M", updatedCartItem.SelectedSizes);
        Assert.Contains("L", updatedCartItem.SelectedSizes);
        Assert.Equal(2, updatedCartItem.SelectedSizes.Count);
    }

    [Fact]
    public async Task UserHasItemsInCart_ShouldReturnTrue_WhenUserHasItems()
    {
        var userId = User!.Id.ToString();

        bool result = await shoppingCartService.UserHasItemsInCart(userId);

        Assert.True(result);
    }

    [Fact]
    public async Task UserHasItemsInCart_ShouldReturnFalse_WhenUserDoesNotExists()
    {
        var notExistingUserId = Guid.NewGuid().ToString();

        bool result = await shoppingCartService.UserHasItemsInCart(notExistingUserId);

        Assert.False(result);
    }

    [Fact]
    public async Task UserHasItemsInCart_ShouldReturnFalse_WhenUserDoesIdIsStringEmpty()
    {
        var notExistingUserId = string.Empty;

        bool result = await shoppingCartService.UserHasItemsInCart(notExistingUserId);

        Assert.False(result);
    }
}