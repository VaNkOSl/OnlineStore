namespace OnlineStore.Tests.Users;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using OnlineStore.Areas.Admin.Controllers;
using OnlineStore.Commons;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.Admin;
using System.Security.Claims;
using static OnlineStore.Commons.MessagesConstants;

public class UserControllerTests
{
    private readonly Mock<IUserService> userServiceMock;
    private readonly Mock<IMemoryCache> memoryCacheMock;
    private readonly UserController userController;

    public UserControllerTests()
    {
        userServiceMock = new Mock<IUserService>();
        memoryCacheMock = new Mock<IMemoryCache>();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "testUserId")
        }, "mock"));

        userController = new UserController(userServiceMock.Object, memoryCacheMock.Object);

        userController.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        userController.TempData = new TempDataDictionary(
          userController.HttpContext,
          Mock.Of<ITempDataProvider>());
    }

    [Fact]
    public async Task All_ShouldReturnViewWithUsersFromCache_WhenUsersAreCached()
    {
        var cachedUsers = GetTestUsers();
        object cacheEntry = cachedUsers;
        memoryCacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheEntry))
                       .Returns(true);

        var result = await userController.All();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<UserViewModel>>(viewResult.ViewData.Model);
        Assert.Equal(cachedUsers, model);

        userServiceMock.Verify(x => x.GetAllUsersAsync(), Times.Never);
    }

    [Fact]
    public async Task BlockUser_ShouldReturnViewWithModel_WhenUserExists()
    {
        var userId = "test-user-id";
        var userModel = GetTestUserViewModel();

        userServiceMock
            .Setup(s => s.GetUserToBlockAsync(userId))
            .ReturnsAsync(userModel);

        var result = await userController.BlockUser(userId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<BlockUserViewModel>>(viewResult.ViewData.Model);
        Assert.Equal(userModel, model);
    }

    [Fact]
    public async Task BlockUser_ShouldReturnError404_WhenUserDoesNotExist()
    {
        var userId = "non-existent-user-id";
        userServiceMock
            .Setup(s => s.GetUserToBlockAsync(userId))
            .ReturnsAsync(new List<BlockUserViewModel>());

        var result = await userController.BlockUser(userId);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("~/Views/Home/Error404.cshtml", viewResult.ViewName);
    }

    [Fact]
    public async Task BlockUser_Post_ShouldRedirectToAllUser_WhenUserDoesNotExists()
    {
        var notExistingUserId = Guid.NewGuid().ToString();

        userServiceMock
            .Setup(s => s.UserExistsAsync(notExistingUserId))
            .ReturnsAsync(false);

        var model = new BlockUserViewModel{ Id = notExistingUserId };

        var result = await userController.BlockUser(notExistingUserId, model);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("All", redirectResult.ActionName);
        Assert.Equal("User", redirectResult.ControllerName);
        Assert.Equal(UserNotFound, userController.TempData[NotificationMessagesConstants.ErrorMessage]);
    }

    [Fact]
    public async Task BlockUser_Post_ShouldBlockUserAndRedirecttoAllUser_WhenUserExists()
    {
        var existingUserId = Guid.NewGuid().ToString();

        userServiceMock
            .Setup(s => s.UserExistsAsync(existingUserId))
            .ReturnsAsync(true);

        userServiceMock
            .Setup(s => s.BlockUserAsync(existingUserId))
            .Returns(Task.CompletedTask);

        var model = new BlockUserViewModel {  Id = existingUserId ,Email = "existingEmail@abv.bg",UserFullName = "Ivan Petrov"};

        var result = await userController.BlockUser(existingUserId, model);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("All", redirectResult.ActionName);
        Assert.Equal("User", redirectResult.ControllerName);
        Assert.Equal(string.Format(SuccessfullyBlockAUser, model.UserFullName), userController.TempData[NotificationMessagesConstants.WarningMessage]);
    }

    [Fact]
    public async Task BlockUser_Post_ShouldRedirectToHomeDashBoard_WhenExceptionIsThrown()
    {
        var existingUserId = Guid.NewGuid().ToString();

        userServiceMock
            .Setup(s => s.UserExistsAsync(existingUserId))
            .ReturnsAsync(true);

        userServiceMock
            .Setup(s => s.BlockUserAsync(existingUserId))
            .ThrowsAsync(new Exception());

        var model = new BlockUserViewModel { Id = existingUserId, Email = "existingEmail@abv.bg", UserFullName = "Ivan Petrov" };

        var result = await userController.BlockUser(existingUserId, model);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("DashBoard", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
        Assert.Equal(string.Format(UnexpectedErrorWhileBlockinUser, model.UserFullName), userController.TempData[NotificationMessagesConstants.ErrorMessage]);
    }

    private IEnumerable<UserViewModel> GetTestUsers()
    {
        return new List<UserViewModel>
        {
            new UserViewModel { Id = "1", Email = "user1@test.com", FullName = "User One" },
            new UserViewModel { Id = "2", Email = "user2@test.com", FullName = "User Two" }
        };
    }

    private IEnumerable<BlockUserViewModel> GetTestUserViewModel()
    {
        return new List<BlockUserViewModel>
        {
            new BlockUserViewModel
            {
                Id = "test-user-id",
                Email = "test@test.com",
                UserFullName = "Test User"
            }
        };
    }
}
