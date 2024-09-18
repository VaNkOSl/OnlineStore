namespace OnlineStore.Tests.Sellers;

using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Data.Data.Common;
using OnlineStore.Services.Data;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.Admin;
using OnlineStore.Web.ViewModels.Sellers;
using static DataBaseSeeder;

public class SellersServiceTests
{
    private DbContextOptions<OnlineStoreDbContext> dbOptions;
    private OnlineStoreDbContext dbContext;

    private readonly ISellerService sellerService;

    public SellersServiceTests()
    {
        dbOptions = new DbContextOptionsBuilder<OnlineStoreDbContext>()
            .UseInMemoryDatabase("OnlineStoreInMemory" + Guid.NewGuid().ToString())
            .Options;

        dbContext = new OnlineStoreDbContext(dbOptions);
        dbContext.Database.EnsureCreated();
        SeedDataBase(dbContext);

        IRepository repository = new Repository(dbContext);
        
        sellerService = new SellerService(repository);
    }

    [Fact]
    public async Task UserExistsById_ShouldReturnTrueWhenExists()
    {
        string existingSellerUserId = SellerUser!.Id.ToString();

        bool result = await sellerService.ExistsByIdAsync(existingSellerUserId);

        Assert.True(result);
    }

    [Fact]
    public async Task UserExistsById_ShouldReturnFalseWhenUserDoesNotExists()
    {
        var notExistingSellerUserId = Guid.NewGuid();

        bool result = await sellerService.ExistsByIdAsync(notExistingSellerUserId.ToString());

        Assert.False(result);
    }

    [Fact]
    public async Task UserExistsById_ShouldReturnFalseWhenUserIdIsStringEmpty()
    {
        var notExistingSellerUserId = string.Empty;

        bool result = await sellerService.ExistsByIdAsync(notExistingSellerUserId.ToString());

        Assert.False(result);
    }

    [Fact]
    public async Task GetSellerByIdAsync_ShouldReturnCorrecrSellerWhenSellerExists()
    {
        var expectedSellerId = SellerUser!.Id.ToString();

        string sellerId = await sellerService.GetSellerByIdAsync(Seller!.UserId.ToString());

        Assert.NotNull(sellerId);
        Assert.NotEqual(expectedSellerId, sellerId); 
    }

    [Fact]
    public async Task GetSellerByIdAsync_ShouldReturnFalseWhenSellerDoesNotExists()
    {
        var nonExistingSellerId = Guid.NewGuid();

        string sellerId = await sellerService.GetSellerByIdAsync(nonExistingSellerId.ToString());

        Assert.Null(sellerId);
    }

    [Fact]
    public async Task GetSellerByIdAsync_ShouldReturnFalseWhenSellerIdIsStringEmpty()
    {
        var nonExistingSellerId = string.Empty;

        string sellerId = await sellerService.GetSellerByIdAsync(nonExistingSellerId.ToString());

        Assert.Null(sellerId);
    }

    [Fact]
    public async Task CreateSeller_ShouldAddedToDataBaseSuccessfully()
    {
        var sellerCountBefore = dbContext.Sellers.Count();

        var model = new SellerFormModel
        {
            FirstName = "test",
            LastName = "Testov",
            PhoneNumber = "+123456789",
            Description = "Test Test Test Test Test Test",
            DateOfBirth = new DateTime(1990, 1, 1),
            Egn = "1234567890"
        };

        var userId = Guid.NewGuid().ToString();

        await sellerService.CreateSellerAsync(model, userId);

        var countAfterAdding = dbContext.Sellers.Count();

        Assert.Equal(countAfterAdding, sellerCountBefore + 1);

        var seller = await dbContext.Sellers.FirstOrDefaultAsync(s => s.UserId.ToString() == userId);

        Assert.NotNull(seller);
        Assert.Equal(seller.FirstName, model.FirstName);
        Assert.Equal(seller.LastName, model.LastName);
        Assert.Equal(seller.PhoneNumber, model.PhoneNumber);
        Assert.Equal(seller.Description, model.Description);
        Assert.Equal(seller.DateOfBirth, model.DateOfBirth);
        Assert.Equal(seller.Egn, model.Egn);
    }

    [Fact]
    public async Task SellerWithEgnAlredyExistsAsync_ShouldReturnTrueWhenExists()
    {
        string existingSellerEgn = "0341815267";

        bool result = await sellerService.SellerWithEgnAlredyExistsAsync(existingSellerEgn);

        Assert.True(result);
    }

    [Fact]
    public async Task SellerWithEgnAlredyExistsAsync_ShouldReturnFalseWhenDoesNotExists()
    {
        string notExistingSellerEgn = "0341815277";

        bool result = await sellerService.SellerWithEgnAlredyExistsAsync(notExistingSellerEgn);

        Assert.False(result);
    }

    [Fact]
    public async Task SellerWithPhoneNumberAlredyExistsAsync_ShouldReturnTrueWhenExists()
    {
        string existingPhoneNumber = "+3591478569";

        bool result = await sellerService.SellerWithPhoneNumberAlredyExistsAsync(existingPhoneNumber);

        Assert.True(result);
    }

    [Fact]
    public async Task SellerWithPhoneNumberAlredyExistsAsync_ShouldReturnFalseWhenDoesNotExists()
    {
        string notExistingPhoneNumber = "+35977777777";

        bool result = await sellerService.SellerWithPhoneNumberAlredyExistsAsync(notExistingPhoneNumber);

        Assert.False(result);
    }

    [Fact]
    public async Task SellerHasProductsAsync_ShouldReturnTrueWhenSellerHasProducts()
    {
        var existingSellerId = Seller!.UserId.ToString();
        var existingProductId = Product!.Id.ToString();

        bool result = await sellerService.SellerHasProductsAsync(existingSellerId, existingProductId);

        Assert.True(result);
    }

    [Fact]
    public async Task SellerHasProductsAsync_ShouldReturnFalseWhenSellerDoesNotHasProducts()
    {
        var notExistingSellerId = Guid.NewGuid().ToString();
        var notExistingProductId = Guid.NewGuid().ToString();

        bool result = await sellerService.SellerHasProductsAsync(notExistingSellerId, notExistingProductId);

        Assert.False(result);
    }

    [Fact]
    public async Task SellerHasProductsAsync_ShouldReturnFalseWhenSellerIdAndProductIdAreStringEmpty()
    {
        var notExistingSellerId = string.Empty;
        var notExistingProductId = string.Empty;

        bool result = await sellerService.SellerHasProductsAsync(notExistingSellerId, notExistingProductId);

        Assert.False(result);
    }

    [Fact]
    public async Task ApproveSellerAsync_ShouldApproveSellerWhenNotApproved()
    {
        var notApprovedSellerId = NotApprovedSeller!.Id.ToString();

        await sellerService.ApproveSellerAsync(notApprovedSellerId);

        var seller = await dbContext.Sellers.FindAsync(Guid.Parse(notApprovedSellerId));

        Assert.NotNull(seller);
        Assert.True(seller.IsApproved);
    }

    [Fact]
    public async Task ApproveSellerAsync_ShouldNotChangeSellerWhenAlreadyApproved()
    {
        var seller = await dbContext.Sellers.FindAsync(NotApprovedSeller!.Id);
        seller!.IsApproved = true;
        await dbContext.SaveChangesAsync();

        await sellerService.ApproveSellerAsync(NotApprovedSeller.Id.ToString());

        var updatedSeller = await dbContext.Sellers.FindAsync(NotApprovedSeller.Id);
        Assert.NotNull(updatedSeller);
        Assert.True(updatedSeller.IsApproved);
    }

    [Fact]
    public async Task RejectSellerAsync_ShouldRejectSellerWithCorrectReason()
    {
        var rejecReason = "Not meeting the criteria!";
        var sellerId = NotApprovedSeller!.Id.ToString();

        var model = new RejectSellerFormModel
        {
          RejectionReason = rejecReason
    
        };


        await sellerService.RejectSellerAsync(model,sellerId);

        var seller = await dbContext.Sellers.FirstOrDefaultAsync(s => s.Id.ToString() == sellerId);

        Assert.NotNull(seller);
        Assert.False(seller.IsApproved);
        Assert.True(seller.IsAdminReject);
        Assert.Equal(rejecReason, seller.RejectionReason);
    }

    [Fact]
    public async Task RejectSellerAsync_ShouldNotRejectAlreadyApprovedSeller()
    {
        var seller = await dbContext.Sellers.FindAsync(Seller!.Id);
        var rejecReason = "Not meeting the criteria!";

        var model = new RejectSellerFormModel
        {
            RejectionReason = rejecReason

        };

        var updatedSeller = await dbContext.Sellers.FindAsync(Seller!.Id);
        Assert.NotNull(updatedSeller);
        Assert.True(updatedSeller!.IsApproved);
        Assert.False(updatedSeller.IsAdminReject);
        Assert.Null(updatedSeller.RejectionReason);
    }

    [Fact]
    public async Task IsUserApprovedAsync_ShouldReturnTrueIfUserIsApproved()
    {
        var approvedSellerId = Seller!.UserId.ToString();

        bool result = await sellerService.IsUserApprovedAsync(approvedSellerId);

        Assert.True(result);
    }

    [Fact]
    public async Task IsUserApprovedAsync_ShouldReturnFalseWhenUserIsNotApproved()
    {
        var notApprovedSellerId = NotApprovedSeller!.UserId.ToString();

        bool result = await sellerService.IsUserApprovedAsync(notApprovedSellerId);

        Assert.False(result);
    }

    [Fact]
    public async Task IsUserApprovedAsync_ShouldReturnFalseWhenUserIdIsStringEmpty()
    {
        var notExistingUserId = string.Empty;

        bool result = await sellerService.IsUserApprovedAsync(notExistingUserId);

        Assert.False(result);
    }

    [Fact]
    public async Task GetSellerByNameAsync_ShouldReturnCorrectSellerName_WhenSellerExists()
    {
        string expectedSellerName = "Ivan Petrov";

        string result = await sellerService.GetSellerByNameAsync(Seller!.Id.ToString());

        Assert.NotNull(result);
        Assert.Equal(expectedSellerName, result);   
    }

    [Fact]
    public async Task GetSellerByNameAsync_ShouldReturnUnknownSellerName_WhenSellerDoesNotExist()
    {
        string nonExistingSellerId = Guid.NewGuid().ToString();

        string result = await sellerService.GetSellerByNameAsync(nonExistingSellerId);

        Assert.Equal("Unknown seller name", result);
    }

    [Fact]
    public async Task GetRejectionReasonAsync_ShouldReturnCorrectRejectReason_WhenSellerExists()
    {
        var expectedRejectReason = "Test reject reason";

        string result = await sellerService.GetRejectionReasonAsync(NotApprovedSeller!.UserId.ToString());

        Assert.Equal(expectedRejectReason, result);
    }

    [Fact]
    public async Task GetRejectionReasonAsync_ShouldReturnNoReasonForYourRejectingProvided_WhenSellerDoesNotExist()
    {
        string nonExistingSellerId = Guid.NewGuid().ToString();

        string result = await sellerService.GetRejectionReasonAsync(nonExistingSellerId);

        Assert.Equal("No reason for your rejecting provided!", result);
    }

    [Fact]
    public async Task IsAdminRejectedAsync_ShouldReturnTrue_WhenAdminIsRejected()
    {
        bool result = await sellerService.IsAdminRejectedAsync(NotApprovedSeller!.UserId.ToString());

        Assert.True(result);
    }

    [Fact]
    public async Task IsAdminRejectedAsync_ShouldReturnFalse_WhenAdminIsNotrejected()
    {
        bool result = await sellerService.IsAdminRejectedAsync(Seller!.UserId.ToString());

        Assert.False(result);
    }

    [Fact]
    public async Task IsAdminRejectedAsync_ShouldReturnFalse_WhenUserIdDoesNotExists()
    {
        var notExistingUserId = Guid.NewGuid().ToString();

        bool result = await sellerService.IsAdminRejectedAsync(notExistingUserId);

        Assert.False(result);
    }

    [Fact]
    public async Task SellerEmailExistsAsync_ShouldReturnTrue_WhenEmailExistsAndIsLinkedToSeller()
    {
        string existingEmail = "pesho@seller.com";
        string userId = "93e72464-4832-4483-952f-41d221ab1091";

        bool result = await sellerService.SellerEmailExistsAsync(userId, existingEmail);

        Assert.True(result);
    }

    [Fact]
    public async Task SellerEmailExistsAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        string nonExistingEmail = "nonexisting@seller.com";
        string userId = "93e72464-4832-4483-952f-41d221ab1091";

        bool result = await sellerService.SellerEmailExistsAsync(userId, nonExistingEmail);

        Assert.False(result);
    }
}