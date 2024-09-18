namespace OnlineStore.Tests.Sellers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using OnlineStore.Commons;
using OnlineStore.Controllers;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.Sellers;
using System.Security.Claims;
using static OnlineStore.Commons.MessagesConstants;

public class SellersControllerTests
{
    private readonly Mock<ISellerService> mockSellerService;
    private readonly Mock<IUserService> mockUserService;
    private readonly SellerController controller;

    public SellersControllerTests()
    {
        mockSellerService = new Mock<ISellerService>();
        mockUserService = new Mock<IUserService>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "testUserId")
        }, "mock"));

        controller = new SellerController(mockSellerService.Object, mockUserService.Object);
        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext { User = user}
        };

         controller.TempData = new TempDataDictionary(
         controller.HttpContext,
         Mock.Of<ITempDataProvider>());
    }

    [Fact]
    public async Task Become_Get_ShouldRedirectToUserNotifications_WhenAdminIsRejected()
    {
        mockSellerService
            .Setup(s => s.IsAdminRejectedAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await controller.Become();
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("UserNotifications", redirectResult.ActionName);
        Assert.Equal("Notification", redirectResult.ControllerName);
        Assert.Equal(AdminIsRejected, controller.TempData[NotificationMessagesConstants.ErrorMessage]);
    }

    [Fact]
    public async Task Become_Get_ShouldRedirectToAllProduct_WhenUserAlreadyIsSeller()
    {
        mockSellerService
          .Setup(s => s.IsAdminRejectedAsync(It.IsAny<string>()))
          .ReturnsAsync(false);

        mockUserService.
            Setup(u => u.GetUserFullNameAsync(It.IsAny<string>()))
            .ReturnsAsync("Ivan Petrov");

        mockSellerService.
            Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await controller.Become();
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("All", redirectResult.ActionName);
        Assert.Equal("Product", redirectResult.ControllerName);
        Assert.Equal(string.Format(UserIsAlreadySeller, "Ivan Petrov"), controller.TempData[NotificationMessagesConstants.ErrorMessage]);
    }

    [Fact]
    public async Task Become_Get_ShouldReturnViewResult_WhenUsercanBeASeller()
    {
        mockSellerService
          .Setup(s => s.IsAdminRejectedAsync(It.IsAny<string>()))
          .ReturnsAsync(false);

        mockSellerService.
           Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
           .ReturnsAsync(false);

        var result = await controller.Become();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Become_Post_ShouldReturnViewWithError_WhenSellerWithSameEgnExists()
    {
        var model = new SellerFormModel();

        mockSellerService
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        mockSellerService
            .Setup(s => s.SellerWithEgnAlredyExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await controller.Become(model);
        var viewResult = Assert.IsType<ViewResult>(result);

        Assert.Equal(model, viewResult.Model);
        Assert.True(controller.ModelState.ContainsKey(nameof(model.Egn)));
    }

    [Fact]
    public async Task Become_Post_ShouldReturnViewWithError_WhenSellerWithSamePhoneNumberExists()
    {
        var model = new SellerFormModel();

        mockSellerService
         .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
         .ReturnsAsync(false);

        mockSellerService
            .Setup(s => s.SellerWithEgnAlredyExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        mockSellerService.
            Setup(s => s.SellerWithPhoneNumberAlredyExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await controller.Become(model);
        var viewResult = Assert.IsType<ViewResult>(result);

        Assert.Equal(model,viewResult.Model);
        Assert.True(controller.ModelState.ContainsKey(nameof(model.PhoneNumber)));
    }

    [Fact]
    public async Task Become_Post_ShouldReturnView_WhenModelStateIsInvalid()
    {
        var model = new SellerFormModel();

        controller.ModelState.AddModelError("Error", "Model state is invalid");

        mockSellerService
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        mockSellerService
            .Setup(s => s.SellerWithEgnAlredyExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        mockSellerService
            .Setup(s => s.SellerWithPhoneNumberAlredyExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await controller.Become(model);
        var viewResult = Assert.IsType<ViewResult>(result);

        Assert.Equal(model, viewResult.Model);
        Assert.True(controller.TempData.ContainsKey(NotificationMessagesConstants.ErrorMessage));
        Assert.Equal(InvalidModel, controller.TempData[NotificationMessagesConstants.ErrorMessage]);
    }

    [Fact]
    public async Task Become_Post_ShouldRedirectToHomeIndex_WhenSellerIsCreatedSuccessfully()
    {
        var model = new SellerFormModel();

        mockSellerService
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        mockSellerService
          .Setup(s => s.SellerWithEgnAlredyExistsAsync(It.IsAny<string>()))
          .ReturnsAsync(false);

        mockSellerService
            .Setup(s => s.SellerWithPhoneNumberAlredyExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        mockSellerService
            .Setup(s => s.CreateSellerAsync(model, It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var result = await controller.Become(model);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal(nameof(HomeController.Index), redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
        Assert.Equal(ApplicationSuccessfully, controller.TempData[NotificationMessagesConstants.SuccessMessage]);
    }

    [Fact]
    public async Task Become_Post_ShouldRedirectToHomeIndex_WhenExceptionIsThrown()
    {
        var model = new SellerFormModel();

        mockSellerService
            .Setup(s => s.ExistsByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        mockSellerService
          .Setup(s => s.SellerWithEgnAlredyExistsAsync(It.IsAny<string>()))
          .ReturnsAsync(false);

        mockSellerService
            .Setup(s => s.SellerWithPhoneNumberAlredyExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        mockSellerService
          .Setup(s => s.CreateSellerAsync(model, It.IsAny<string>()))
          .ThrowsAsync(new Exception());


        var result = await controller.Become(model);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal(nameof(HomeController.Index), redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
        Assert.Equal(UnexpectedErrorOccurredCreatingSeller, controller.TempData[NotificationMessagesConstants.ErrorMessage]);
    }
}
