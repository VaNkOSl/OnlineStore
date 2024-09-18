namespace OnlineStore.Tests.Users;

using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Data.Data.Common;
using OnlineStore.Services.Data;
using OnlineStore.Services.Data.Contacts;
using static DataBaseSeeder;

public class UsersServiceTests
{
    private DbContextOptions<OnlineStoreDbContext> dbOptions;
    private OnlineStoreDbContext dbContext;

    private IUserService userService;

    public UsersServiceTests()
    {
        dbOptions = new DbContextOptionsBuilder<OnlineStoreDbContext>()
             .UseInMemoryDatabase("OnlineStoreDbContext" + Guid.NewGuid().ToString())
             .Options;

        dbContext = new OnlineStoreDbContext(dbOptions);
        dbContext.Database.EnsureCreated();
        SeedDataBase(dbContext);

        IRepository repository = new Repository(dbContext);

        userService = new UserService(repository);
    }

    [Fact]
    public async Task GetUserFullNameAsync_ShouldReturnUserFullName_WhenUserExists()
    {
        var userId = NotApprovedSellerUser!.Id.ToString();

        var result = await userService.GetUserFullNameAsync(userId);

        Assert.NotNull(result);
        Assert.Equal("Ivan Petrov", result);
    }

    [Fact]
    public async Task GetUserFullNameAsync_ShouldReturnUnknownUser_WhenUserDoesNotExists()
    {
        var userId = Guid.NewGuid().ToString();

        var result = await userService.GetUserFullNameAsync(userId);

        Assert.NotNull(result);
        Assert.Equal("Unknown user", result);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsersWithCorrectDetails()
    {
        var result = await userService.GetAllUsersAsync();

        Assert.NotNull(result);
        Assert.Equal(4, result.Count());

        var firstUser = result.FirstOrDefault(u => u.Email == "pesho@seller.com");
        var secondUser = result.FirstOrDefault(u => u.Email == "ivan@user.com");

        Assert.NotNull(firstUser);
        Assert.True(firstUser.IsSeller);
        Assert.Equal("+3591478569", firstUser.PhoneNumber);
        Assert.NotNull(secondUser);
    }

    [Fact]
    public async Task BlockUserAsync_ShouldBlockUserSuccessfully_WhenUserExists()
    {
         var userId = NotApprovedSellerUser!.Id.ToString();

         await userService.BlockUserAsync(userId);

         Assert.True(NotApprovedSellerUser.IsBlocked);
    }

    [Fact]  
    public async Task BlockUserAsync_ShoudNotBlockUser_WhenUserDoesNotExists()
    {
        var nonExistentUserId = Guid.NewGuid().ToString();

        await userService.BlockUserAsync(nonExistentUserId);

        var result = await dbContext.Users.AnyAsync(u => u.Id.ToString() == nonExistentUserId);
        Assert.False(result);  
    }

    [Fact]
    public async Task GetUserToBlockAsync_ShouldReturnCorrecrUserToBlock_WhenUserExists()
    {
        var userId = NotApprovedSellerUser!.Id.ToString();

        var result = await userService.GetUserToBlockAsync(userId);

        var firstUser = result.FirstOrDefault(u => u.Email == "ivan@user.com");

        Assert.NotNull(result);
        Assert.Equal("ivan@user.com", firstUser!.Email);
        Assert.Equal("Ivan Petrov", firstUser.UserFullName);
    }

    [Fact]
    public async Task UserExistsAsync_ShouldReturnTrue_WhenUserExists()
    {
        var userId = NotApprovedSellerUser!.Id.ToString();

        bool result = await userService.UserExistsAsync(userId);

        Assert.True(result);
    }

    [Fact]
    public async Task UserExistsAsync_ShouldReturnFalse_WhenUserDoesNotExists()
    {
        var notExistingUserId = Guid.NewGuid().ToString();

        bool result = await userService.UserExistsAsync(notExistingUserId);

        Assert.False(result);
    }

    [Fact]
    public async Task UserExistsAsync_ShouldReturnFalse_WhenUserIdIsStringEmpty()
    {
        var userId = string.Empty;

        bool result = await userService.UserExistsAsync(userId);

        Assert.False(result);
    }

    [Fact]
    public async Task UserEmailExistsAsync_ShouldReturnTrue_WhenUserEmailExists()
    {
        var userEmail = NotApprovedSellerUser!.Email;

        bool result = await userService.UserEmailExistsAsync(userEmail!);

        Assert.True(result);
    }

    [Fact]
    public async Task UserEmailExistsAsync_ShouldReturnFalse_WhenUserEmailDoesNotExists()
    {
        var notExistingUserEmail = "notExistinEmail@abv.bg";

        bool result = await userService.UserEmailExistsAsync(notExistingUserEmail!);

        Assert.False(result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnCorrectUserEmail_WhenUserExists()
    {
        var email = NotApprovedSellerUser!.Email!.ToString();

        var result = await userService.GetUserByEmailAsync(email);

        Assert.Equal("ivan@user.com", result!.Email);
    }
}
