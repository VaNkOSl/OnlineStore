namespace OnlineStore.Tests.IntegrationTesting;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Data;
using OnlineStore.Tests.TestAuthentications;
using OnlineStore.Web.ViewModels.Sellers;
using System.Net;
using System.Text.RegularExpressions;

public class SellerIntegrationTesting : IClassFixture<IntegrationTestFactory<Program>>
{
    private readonly IntegrationTestFactory<Program> _factory;
    private HttpClient client;
    private OnlineStoreDbContext _dbContext;

    public SellerIntegrationTesting(IntegrationTestFactory<Program> factory)
    {
        _factory = factory;
        client = BuildAuthenticatedUserClient();
        InitializeDbContext();
    }

    [Fact]
    public async Task Become_Get_ShouldReturnViewModelForBecomeASeller_WhenSellerDoesNotExists()
    {
        var response = await client.GetAsync("/Seller/Become");

        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

        Assert.Contains("<h2 class=\"text-center\">Become Seller</h2>", responseAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"FirstName\">First Name:</label>", responseAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"LastName\">Last Name:</label>", responseAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"Description\">Description:</label>", responseAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"PhoneNumber\">Phone Number:</label>", responseAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"DateOfBirth\">Date of Birth:</label>", responseAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"Egn\">EGN:</label>", responseAsString);
    }

    [Fact]
    public async Task Become_Post_ShouldCreateSellerSuccessfully_WhenDataIsValid()
    {
        var model = new SellerFormModel
        {
            FirstName = "John",
            LastName = "Doein",
            Description = "Experienced seller.This is test for John Doein",
            PhoneNumber = "1234567890",
            DateOfBirth = new DateTime(1990, 1, 1),
            Egn = "1234567890"
        };

        var responseGet = await client.GetAsync("/Seller/Become");
        responseGet.EnsureSuccessStatusCode();

        var responseAsString = await responseGet.Content.ReadAsStringAsync();

        var tokenMatch = Regex.Match(responseAsString, @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" />");
        var requestVerificationToken = tokenMatch.Success ? tokenMatch.Groups[1].Value : string.Empty;

        Assert.False(string.IsNullOrEmpty(requestVerificationToken), "CSRF token not found.");

        var formData = new Dictionary<string, string>
        {
            { nameof(SellerFormModel.FirstName), model.FirstName },
            { nameof(SellerFormModel.LastName), model.LastName },
            { nameof(SellerFormModel.Description), model.Description },
            { nameof(SellerFormModel.PhoneNumber), model.PhoneNumber },
            { nameof(SellerFormModel.DateOfBirth), model.DateOfBirth.ToString("dd/MM/yyyy") }, 
            { nameof(SellerFormModel.Egn), model.Egn },
            { "__RequestVerificationToken", requestVerificationToken } 
        };

        var content = new FormUrlEncodedContent(formData);

        var response = await client.PostAsync("/Seller/Become", content);

        var responsePostAsString = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain("InvalidModel", responsePostAsString);
        Assert.DoesNotContain("UnexpectedErrorOccurredCreatingSeller", responsePostAsString);
    }

    [Fact]
    public async Task Become_Post_ShouldReturnViewWithErrors_WhenModelIsInvalid()
    {
        var model = new SellerFormModel
        {
            FirstName = "",
            LastName = "Doein",
            Description = "Experienced seller.This is test for John Doein",
            PhoneNumber = "1234567890",
            DateOfBirth = new DateTime(1990, 1, 1),
            Egn = "1234567890"
        };

        var responseGet = await client.GetAsync("/Seller/Become");
        responseGet.EnsureSuccessStatusCode();

        var responseAsString = await responseGet.Content.ReadAsStringAsync();

        var tokenMatch = Regex.Match(responseAsString, @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" />");
        var requestVerificationToken = tokenMatch.Success ? tokenMatch.Groups[1].Value : string.Empty;

        Assert.False(string.IsNullOrEmpty(requestVerificationToken), "CSRF token not found.");

        var formData = new Dictionary<string, string>
        {
            { nameof(SellerFormModel.FirstName), model.FirstName },
            { nameof(SellerFormModel.LastName), model.LastName },
            { nameof(SellerFormModel.Description), model.Description },
            { nameof(SellerFormModel.PhoneNumber), model.PhoneNumber },
            { nameof(SellerFormModel.DateOfBirth), model.DateOfBirth.ToString("dd/MM/yyyy") },
            { nameof(SellerFormModel.Egn), model.Egn },
            { "__RequestVerificationToken", requestVerificationToken }
        };

        var content = new FormUrlEncodedContent(formData);

        var response = await client.PostAsync("/Seller/Become", content);

        response.StatusCode = HttpStatusCode.OK;

        var responsePostAsString = await response.Content.ReadAsStringAsync();

        Assert.Contains("<h2 class=\"text-center\">Become Seller</h2>", responsePostAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"FirstName\">First Name:</label>", responsePostAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"LastName\">Last Name:</label>", responsePostAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"Description\">Description:</label>", responsePostAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"PhoneNumber\">Phone Number:</label>", responsePostAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"DateOfBirth\">Date of Birth:</label>", responsePostAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"Egn\">EGN:</label>", responsePostAsString);
        Assert.Contains("<script>toastr.error('Please correct the errors in the form and try again!');</script>", responsePostAsString);
    }

    private HttpClient BuildAuthenticatedUserClient()
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication("TestAuthentication")
                        .AddScheme<AuthenticationSchemeOptions, UserAuthenticationHandler>("TestAuthentication", options => { });
            });
        }).CreateClient();
    }
    private void InitializeDbContext()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            _dbContext = scope.ServiceProvider.GetRequiredService<OnlineStoreDbContext>();
            DataBaseSeeder.SeedDataBase(_dbContext);
        }
    }
}