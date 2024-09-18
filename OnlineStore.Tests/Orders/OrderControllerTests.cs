namespace OnlineStore.Tests.Orders;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using OnlineStore.Controllers;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.Orders;
using System.Security.Claims;
using static OnlineStore.Commons.MessagesConstants;
using static OnlineStore.Commons.NotificationMessagesConstants;


public class OrderControllerTests
{
    private readonly Mock<IOrderService> orderServiceMock;
    private readonly Mock<IProductService> productServiceMock;
    private readonly Mock<ISellerService> sellerServiceMock;
    private readonly Mock<IShoppingCartService> shoppingCartServiceMock;
    private readonly Mock<IProductAttributeService> productAttributeServiceMock;
    private readonly Mock<IUserService> userServiceMock;

    private readonly OrderController controller;

    public OrderControllerTests()
    {
        orderServiceMock = new Mock<IOrderService>();
        productServiceMock = new Mock<IProductService>();
        sellerServiceMock = new Mock<ISellerService>();
        shoppingCartServiceMock = new Mock<IShoppingCartService>();
        productAttributeServiceMock = new Mock<IProductAttributeService>();
        userServiceMock = new Mock<IUserService>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "testUserId")
        },"mock"));

        controller = new OrderController(orderServiceMock.Object, productServiceMock.Object,
                                       productAttributeServiceMock.Object, sellerServiceMock.Object,
                                       shoppingCartServiceMock.Object, userServiceMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        controller.TempData = new TempDataDictionary(
          controller.HttpContext,
          Mock.Of<ITempDataProvider>());
    }

    [Fact]
    public async Task MyOrder_Get_ShouldRedirectToError404_WhenUserDoesNotExists()
    {
        userServiceMock
            .Setup(u => u.UserExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await controller.MyOrder("orderId");

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("~/Views/Home/Error404.cshtml", viewResult.ViewName);
        Assert.Equal(UserNotFound, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task MyOrder_Get_ShouldReturnViewWithOrders_WhenUserExists()
    {
        userServiceMock
             .Setup(u => u.UserExistsAsync(It.IsAny<string>()))
             .ReturnsAsync(true);

        var mockOrders = new List<OrderViewModel>
        {
            new OrderViewModel
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Ivan",
                LastName = "Petrov"
            }
        };

        orderServiceMock
             .Setup(o => o.GetOrderByUserIdAsync(It.IsAny<string>()))
             .ReturnsAsync(mockOrders);

        var result = await controller.MyOrder("orderId");

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<OrderViewModel>>(viewResult.Model);

        Assert.Single(model);
        Assert.Equal("Ivan", model.First().FirstName);
        Assert.Equal("Petrov", model.First().LastName);
    }

    [Fact]
    public async Task MyOrder_Get_ShouldReturnGeneralErrorView_WhenExceptionIsThrown()
    {
        userServiceMock
            .Setup(u => u.UserExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        orderServiceMock
            .Setup(o => o.GetOrderByUserIdAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception());

        var result = await controller.MyOrder("orderId");

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
        Assert.Equal(GeneralErrors, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task CompleteOrder_Get_ShouldRedirectToError404_WhenUserDoesNotExists()
    {
        userServiceMock
            .Setup(u => u.UserExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await controller.CompleteOrder();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("~/Views/Home/Error404.cshtml", viewResult.ViewName);
        Assert.Equal(UserNotFound, controller.TempData[ErrorMessage]);
    }


    [Fact]
    public async Task CompleteOrder_Get_ShouldRedirectToAllProduct_WhenUserHasNothingInCartItem()
    {
        userServiceMock
              .Setup(u => u.UserExistsAsync(It.IsAny<string>()))
              .ReturnsAsync(true);

        shoppingCartServiceMock
            .Setup(s => s.UserHasItemsInCart(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await controller.CompleteOrder();

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("All", redirectResult.ActionName);
        Assert.Equal("Product", redirectResult.ControllerName);
        Assert.Equal(UserHasNoItemInCartItem, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task CompleteOrder_Get_ShouldReturnModelToCompleteOrder_WhenUserExists()
    {
        userServiceMock
             .Setup(u => u.UserExistsAsync(It.IsAny<string>()))
             .ReturnsAsync(true);

        var result = await controller.CompleteOrder();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<OrderFormModel>(viewResult.Model);
    }

    [Fact]
    public async Task CompleteOrder_Post_ShouldReturnViewWithModel_WhenModelStateIsInvalid()
    {
       var model = new OrderFormModel();

        controller.ModelState.AddModelError("Error", "Invalid Model!");

        userServiceMock
            .Setup(u => u.UserExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await controller.CompleteOrder(model);
        var viewResult = Assert.IsType<ViewResult>(result);

        Assert.Equal(model, viewResult.Model);
        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(InvalidModel, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task CompleteOrder_Post_ShouldRedirectToMyOrder_WhenOrderIsSuccessfullyCreated()
    {
        var model = new OrderFormModel();
        var userId = Guid.NewGuid().ToString();
        var orderId = Guid.NewGuid().ToString();

        userServiceMock
        .Setup(u => u.UserExistsAsync(It.IsAny<string>()))
        .ReturnsAsync(true);

        orderServiceMock
            .Setup(o => o.CreateOrderAsync(It.IsAny<OrderFormModel>(),It.IsAny<string>()))
            .ReturnsAsync(orderId);

        shoppingCartServiceMock
            .Setup(s => s.ClearCartItemsAsync())
            .Returns(Task.CompletedTask);

        var result = await controller.CompleteOrder(model);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("MyOrder", resultAsRedirect.ActionName);
        Assert.Equal(orderId, resultAsRedirect.RouteValues?["id"]);
        Assert.True(controller.TempData.ContainsKey(SuccessMessage));
        Assert.Equal(SuccessfullyCompletedOrder, controller.TempData[SuccessMessage]);
    }

    [Fact]
    public async Task CompleteOrder_Post_ShouldRedirectToShoppingCart_WhenExceptionIsThrown()
    {
        var model = new OrderFormModel();
        var userId = Guid.NewGuid().ToString();

        userServiceMock
           .Setup(u => u.UserExistsAsync(It.IsAny<string>()))
           .ReturnsAsync(true);

        orderServiceMock
            .Setup(o => o.CreateOrderAsync(It.IsAny<OrderFormModel>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception());

        var result = await controller.CompleteOrder(model);
        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("CartItem", resultAsRedirect.ActionName);
        Assert.Equal("ShoppingCart",resultAsRedirect.ControllerName);

        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(UnexpectedErrorOccurredCompleteOrder, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task OrdersForProduct_Get_ShouldRedirectToSellerBecome_WhenUserDoesNotExists()
    {
        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await controller.OrdersForProduct();

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Become", resultAsRedirect.ActionName);
        Assert.Equal("Seller", resultAsRedirect.ControllerName);

        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(UserNotASeller, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task OrdersForProduct_Get_ShouldRedirectToError401_WhenUserIsNotASeller()
    {
        sellerServiceMock
           .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
           .ReturnsAsync(true);

        sellerServiceMock?
          .Setup(s => s.GetSellerByIdAsync(It.IsAny<string>()))
          .ReturnsAsync((string?)null);

        var result = await controller.OrdersForProduct();
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("~/Views/Home/Error401.cshtml", viewResult.ViewName);

        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(SellerNotFound, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task OrdersForProduct_Get_ShouldReturnViewWithOrders_WhenOrdersAreRetrievedSuccessfully()
    {
        var sellerId = Guid.NewGuid().ToString();

        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        sellerServiceMock
            .Setup(s => s.GetSellerByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(sellerId);

        var mockOrders = new List<OrderViewModel>
        {
            new OrderViewModel { Id = "order1", FirstName = "Ivan", LastName = "Petrov" },
            new OrderViewModel { Id = "order2", FirstName = "Petar", LastName = "Ivanov" }
        };

        orderServiceMock
            .Setup(o => o.GetOrdersByProductAndSellerAsync(It.IsAny<string>()))
            .ReturnsAsync(mockOrders);

        var result = await controller.OrdersForProduct();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(mockOrders, viewResult.Model);
    }

    [Fact]
    public async Task OrdersForProduct_Get_ShouldReturnGeneralError_WhenExceptionIsThrown()
    {
        var sellerId = Guid.NewGuid().ToString();

        sellerServiceMock
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        sellerServiceMock
            .Setup(s => s.GetSellerByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(sellerId);

        orderServiceMock
            .Setup(o => o.GetOrdersByProductAndSellerAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception());

        var result = await controller.OrdersForProduct();

        var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToAction.ActionName);
        Assert.Equal("Home", redirectToAction.ControllerName);

        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(GeneralErrors, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task SendOrder_Post_ShouldRedirectToAllProduct_WhenOrderDoesNotExists()
    {
        var orderId = Guid.NewGuid().ToString();

        orderServiceMock
            .Setup(o => o.OrderExistsAsync(orderId))
            .ReturnsAsync(false);

        var result = await controller.SendOrder(orderId);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("All", resultAsRedirect.ActionName);
        Assert.Equal("Product", resultAsRedirect.ControllerName);

        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(OrderNotFound, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task SendOrder_Post_ShouldRedirectToOrdersForProduct_WhenOrderNotFound()
    {
        var orderId = Guid.NewGuid().ToString();

        orderServiceMock
            .Setup(o => o.OrderExistsAsync(orderId))
            .ReturnsAsync(true);

        orderServiceMock
            .Setup(o => o.SendOrderAsync(orderId))
            .ReturnsAsync(false);

        var result = await controller.SendOrder(orderId);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("OrdersForProduct", resultAsRedirect.ActionName);
        Assert.Equal("Order", resultAsRedirect.ControllerName);

        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(OrderNotFound, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task SendOrder_Post_ShouldPostOrderSuccessfully_WhenOrderExists()
    {
        var orderId = Guid.NewGuid().ToString();

        orderServiceMock
            .Setup(o => o.OrderExistsAsync(orderId))
            .ReturnsAsync(true);

        orderServiceMock
            .Setup(o => o.SendOrderAsync(orderId))
            .ReturnsAsync(true);

        var result = await controller.SendOrder(orderId);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("OrdersForProduct", resultAsRedirect.ActionName);
        Assert.Equal("Order", resultAsRedirect.ControllerName);

        Assert.True(controller.TempData.ContainsKey(SuccessMessage));
        Assert.Equal(SuccessfullySendOrder, controller.TempData[SuccessMessage]);
    }

    [Fact]
    public async Task SendOrder_Post_ShouldReturnUnexpectedErrorOccurredSendOrderAndRedirectToOrdersForProduct_WhenExceptionThrows()
    {
        var orderId = Guid.NewGuid().ToString();

        orderServiceMock
            .Setup(o => o.OrderExistsAsync(orderId))
            .ReturnsAsync(true);

        orderServiceMock
            .Setup(o => o.SendOrderAsync(orderId))
            .ThrowsAsync(new Exception());

        var result = await controller.SendOrder(orderId);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("OrdersForProduct", resultAsRedirect.ActionName);
        Assert.Equal("Order", resultAsRedirect.ControllerName);

        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(UnexpectedErrorOccurredSendOrder, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task TakeOrder_Post_ShouldReturnOrderNotFoundAndRedirectToAllProduct_WhenOrderDoesnotExists()
    {
        var orderId = Guid.NewGuid().ToString();

        orderServiceMock
            .Setup(o => o.OrderExistsAsync(orderId))
            .ReturnsAsync(false);

        var result = await controller.TakeOrder(orderId);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("All", resultAsRedirect.ActionName);
        Assert.Equal("Product", resultAsRedirect.ControllerName);

        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(OrderNotFound, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task TakeOrder_Post_ShouldReturnViewError404_WhenUserDoesNotExists()
    {
        var orderId = Guid.NewGuid().ToString();

        orderServiceMock
            .Setup(o => o.OrderExistsAsync(orderId))
            .ReturnsAsync(true);

        userServiceMock
            .Setup(u => u.UserExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await controller.TakeOrder(orderId);

        var resultAsViewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("~/Views/Home/Error404.cshtml", resultAsViewResult.ViewName);

        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(UserNotFound, controller.TempData[ErrorMessage]);
    }

    [Fact]
    public async Task TakeOrder_Post_ShouldTakeOrderSuccessfully_WhenOrderAndUserExists()
    {
        var orderId = Guid.NewGuid().ToString();

        orderServiceMock
            .Setup(o => o.OrderExistsAsync(orderId))
            .ReturnsAsync(true);

        userServiceMock
            .Setup(u => u.UserExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        orderServiceMock
            .Setup(o => o.TakeOrderAsync(orderId))
            .ReturnsAsync(true);

        var result = await controller.TakeOrder(orderId);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("MyOrder", resultAsRedirect.ActionName);
        Assert.Equal("Order",resultAsRedirect.ControllerName);

        Assert.True(controller.TempData.ContainsKey(SuccessMessage));
        Assert.Equal(SuccessfullyTakeOrder, controller.TempData[SuccessMessage]);
    }

    [Fact]
    public async Task TakeOrder_Post_ShouldReturnUnexpectedErrorOccurredTakeOrder_WhenExceptionThrows()
    {
        var orderId = Guid.NewGuid().ToString();

        orderServiceMock
            .Setup(o => o.OrderExistsAsync(orderId))
            .ReturnsAsync(true);

        userServiceMock
            .Setup(u => u.UserExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        orderServiceMock
            .Setup(o => o.TakeOrderAsync(orderId))
            .ThrowsAsync(new Exception());

        var result = await controller.TakeOrder(orderId);

        var resultAsRedirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("MyOrder", resultAsRedirect.ActionName);
        Assert.Equal("Order", resultAsRedirect.ControllerName);

        Assert.True(controller.TempData.ContainsKey(ErrorMessage));
        Assert.Equal(UnexpectedErrorOccurredTakeOrder, controller.TempData[ErrorMessage]);
    }
}
