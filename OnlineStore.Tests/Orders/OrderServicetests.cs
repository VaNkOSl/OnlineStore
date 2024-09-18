namespace OnlineStore.Tests.Orders;

using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Data.Data.Common;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Services.Data;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.Orders;
using static DataBaseSeeder;

public class OrderServicetests
{
    private DbContextOptions<OnlineStoreDbContext> dbOptions;
    private OnlineStoreDbContext dbContext;

    private readonly IOrderService orderService;

    public OrderServicetests()
    {
        dbOptions = new DbContextOptionsBuilder<OnlineStoreDbContext>()
           .UseInMemoryDatabase("OnlineStoreInMemory" + Guid.NewGuid().ToString())
           .EnableSensitiveDataLogging()
           .Options;

        dbContext = new OnlineStoreDbContext(dbOptions);
        dbContext.Database.EnsureCreated();
        SeedDataBase(dbContext);

        IRepository repository = new Repository(dbContext);

        orderService = new OrderService(repository);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldUpdateOrderAndReturnOrderId_WhenValidDataIsProvided()
    {
        var model = new OrderFormModel
        {
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "123456789",
            Email = "john.doe@example.com",
            DeliveryOption = DeliveryOption.Speedy,
            Adress = "123 Main St"
        };

        var userId = NotApprovedSellerUser!.Id.ToString();

        var orderId = await orderService.CreateOrderAsync(model, userId);

        var order = await dbContext.Orders.Include(o => o.OrderItems)
               .ThenInclude(oi => oi.Product)
               .FirstOrDefaultAsync(o => o.Id.ToString() == orderId);

        Assert.Multiple(() =>
        {
            Assert.NotNull(order);
            Assert.Equal(OrderStatus.Completed, order.OrderStatus);
            Assert.Equal("John", order.FirstName);
            Assert.Equal("Doe", order.LastName);
            Assert.Equal("123456789",order.PhoneNumber);
            Assert.Equal("john.doe@example.com", order.Email);
            Assert.Equal("123 Main St", order.Adress);
        });

        var product = await dbContext.Products.FindAsync(Product!.Id);

        Assert.NotNull(product);
        Assert.Equal(98,product.StockQuantity);
    }

    [Fact]
    public async Task GetOrderByUserIdAsync_ShouldReturnOrders_WhenUserHasOrders()
    {
        var userId = NotApprovedSellerUser!.Id.ToString();

        var orders = await orderService.GetOrderByUserIdAsync(userId);

        Assert.NotNull(orders);
        Assert.NotEmpty(orders);

        var order = orders.First();
        Assert.NotEmpty(order.OrderItems);

        Assert.Equal("John", order.FirstName);
        Assert.Equal("Doe", order.LastName);
        Assert.Equal("123456789", order.PhoneNumber);
        Assert.Equal("john.doe@example.com", order.Email);
        Assert.Equal("123 Main St", order.Adress);

        var orderItem = order.OrderItems.First();
        Assert.NotNull(orderItem);
        Assert.NotEmpty(orderItem.SelectedColors);
        Assert.NotEmpty(orderItem.SelectedSizes);

        Assert.Equal("First Product", orderItem.ProductName);
        Assert.Equal(59.98M, orderItem.Price);  
        Assert.Contains("Blue", orderItem.SelectedColors);
        Assert.Contains("L", orderItem.SelectedSizes);
        Assert.Contains("M", orderItem.SelectedSizes);

        var totalPrice = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);
        Assert.Equal(totalPrice, order.TotalPrice);
    }

    [Fact]
    public async Task GetOrdersByProductAndSellerAsync_ShouldReturnOrders_WhenUserPurchaseOrder()
    {
        var sellerId = Seller!.Id.ToString();

        var orders = await orderService.GetOrdersByProductAndSellerAsync(sellerId);

        Assert.NotNull(orders);
        Assert.NotEmpty(orders);

        var order = orders.First();
        Assert.Equal("John", order.FirstName);
        Assert.Equal("Doe", order.LastName);
        Assert.Equal("123456789", order.PhoneNumber);
        Assert.Equal("john.doe@example.com", order.Email);
        Assert.Equal("123 Main St", order.Adress);
        Assert.Equal(OrderStatus.Cart, order.OrderStatus);

        var orderItem = order.OrderItems.First();
        Assert.NotNull(orderItem);
        Assert.Equal("First Product", orderItem.ProductName);
        Assert.Equal(59.98M, orderItem.Price);
        Assert.Equal(2, orderItem.Quantity);
    }

    [Fact]
    public async Task OrderExistsAsync_ShouldReturnTrue_WhenOrderExists()
    {
        var orderId = Order!.Id.ToString();

        bool result = await orderService.OrderExistsAsync(orderId);

        Assert.True(result);    
    }

    [Fact]
    public async Task OrderExistsAsync_ShouldReturnFalse_WhenOrderDoesNotExists()
    {
        var notExistingOrderId = Guid.NewGuid().ToString();

        bool result = await orderService.OrderExistsAsync(notExistingOrderId);

        Assert.False(result);
    }

    [Fact]
    public async Task OrderExistsAsync_ShouldReturnFalse_WhenOrderIdIsStringEmpty()
    {
        var notExistingOrderId = string.Empty;

        bool result = await orderService.OrderExistsAsync(notExistingOrderId);

        Assert.False(result);
    }
}
