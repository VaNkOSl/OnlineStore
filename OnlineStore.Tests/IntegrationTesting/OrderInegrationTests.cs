namespace OnlineStore.Tests.IntegrationTesting;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Data;
using OnlineStore.Tests.TestAuthentications;
using OnlineStore.Web.ViewModels.Orders;
using System;
using System.Text.RegularExpressions;

public class OrderInegrationTests : IClassFixture<IntegrationTestFactory<Program>>
{
    private readonly HttpClient userClient;
    private readonly HttpClient sellerClient;
    private readonly HttpClient notExistingUserClient;
    private readonly HttpClient notApprovedSellerClient;
    private readonly IntegrationTestFactory<Program> _factory;
    private OnlineStoreDbContext _dbContext;

    public OrderInegrationTests(IntegrationTestFactory<Program> factory)
    {
        _factory = factory;
        userClient = BuildAuthenticatedUserClient();
        notExistingUserClient = BuildAuthenticateNotExistingUserClient();
        notApprovedSellerClient = BuildAuthenticateNotApprovedSellerClinet();
        sellerClient = BuildAuthenticatedSellerClient();

        using (var scope = _factory.Services.CreateScope())
        {
            _dbContext = scope.ServiceProvider.GetRequiredService<OnlineStoreDbContext>();
            DataBaseSeeder.SeedDataBase(_dbContext);
        }
    }

    [Fact]
    public async Task MyOrder_Get_ShouldReturnViewWithOrders_WhenUserExists()
    {
        var response = await userClient.GetAsync("/Order/MyOrder");

        response.EnsureSuccessStatusCode();

     
        var responseAsString = await response.Content.ReadAsStringAsync();

        Assert.Contains("<h2 class=\"text-center\">My Orders</h2>", responseAsString);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task MyOrder_Get_ShoudReturnViewError404_WhenUserDoesNotExists()
    {
        var response = await notExistingUserClient.GetAsync("/Order/MyOrder");

        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("<script>toastr.error('The specified user account could not be found. Please try again later or contact with administrator!');</script>", responseAsString);
        Assert.Contains("<img src=\"https://img.freepik.com/free-vector/404-error-with-landscape-concept-illustration_114360-7888.jpg\" alt=\"Alternative image\" class=\"img-thumbnail\">", responseAsString);
    }

    [Fact]
    public async Task CompleteOrder_Get_ShouldReturnViewToCopleateOrder_WhenUserAndHasProductsInCartItemExists()
    {
        var response = await userClient.GetAsync("/Order/CompleteOrder?");

        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("<h2 class=\"text-center\">Plese fill your data!</h2>", responseAsString);
        Assert.Contains("<label for=\"FirstName\">Enter your first name</label>", responseAsString);
        Assert.Contains("<label for=\"LastName\">Enter your lasst name</label>", responseAsString);
        Assert.Contains("<label for=\"PhoneNumber\">PhoneNumber</label>", responseAsString);
        Assert.Contains("<label for=\"Email\">Email</label>", responseAsString);
        Assert.Contains("<label for=\"Adress\">Adress</label>", responseAsString);
        Assert.Contains("<label for=\"DeliveryOption\">DeliveryOption</label>", responseAsString);
    }

    [Fact]
    public async Task CompleteOrder_Get_ShouldReturnViewError404_WhenUserDoesNotExists()
    {
        var response = await notExistingUserClient.GetAsync("/Order/CompleteOrder?");

        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("<script>toastr.error('The specified user account could not be found. Please try again later or contact with administrator!');</script>", responseAsString);
        Assert.Contains("<img src=\"https://img.freepik.com/free-vector/404-error-with-landscape-concept-illustration_114360-7888.jpg\" alt=\"Alternative image\" class=\"img-thumbnail\">", responseAsString);
        Assert.True(response.IsSuccessStatusCode);
        Assert.DoesNotContain("<h2 class=\"text-center\">Plese fill your data!</h2>", responseAsString);
        Assert.DoesNotContain("<label for=\"FirstName\">Enter your first name</label>", responseAsString);
        Assert.DoesNotContain("<label for=\"LastName\">Enter your lasst name</label>", responseAsString);
        Assert.DoesNotContain("<label for=\"PhoneNumber\">PhoneNumber</label>", responseAsString);
        Assert.DoesNotContain("<label for=\"Email\">Email</label>", responseAsString);
        Assert.DoesNotContain("<label for=\"Adress\">Adress</label>", responseAsString);
        Assert.DoesNotContain("<label for=\"DeliveryOption\">DeliveryOption</label>", responseAsString);
    }

    [Fact]
    public async Task CompleteOrder_Post_ShouldCompleateOrderSuccessfully_WhenUserHasItemsInCart()
    {
        var responseShoppingCart = await userClient.GetAsync("/ShoppingCart/CartItem?");

        responseShoppingCart.EnsureSuccessStatusCode();

        var responseAsStringShoppingCart = await responseShoppingCart.Content.ReadAsStringAsync();

        Assert.True(responseShoppingCart.IsSuccessStatusCode);
        Assert.Contains("<h2>Your Shopping Cart</h2>", responseAsStringShoppingCart);
        Assert.Contains("<th id=\"product\">Product</th>", responseAsStringShoppingCart);
        Assert.Contains("th>Price</th>", responseAsStringShoppingCart);
        Assert.Contains("<th>Quantity</th>", responseAsStringShoppingCart);
        Assert.Contains("<th>Selected Colors</th>", responseAsStringShoppingCart);
        Assert.Contains("<th>Selected Sizes</th>", responseAsStringShoppingCart);
        Assert.Contains("<th>Total</th>", responseAsStringShoppingCart);
        Assert.Contains("<th>Actions</th>", responseAsStringShoppingCart);

        var responseOrder = await userClient.GetAsync("/Order/CompleteOrder?");

        responseOrder.EnsureSuccessStatusCode();
        var responseAsStringOrder = await responseOrder.Content.ReadAsStringAsync();

        Assert.Contains("<h2 class=\"text-center\">Plese fill your data!</h2>", responseAsStringOrder);
        Assert.Contains("<label for=\"FirstName\">Enter your first name</label>", responseAsStringOrder);
        Assert.Contains("<label for=\"LastName\">Enter your lasst name</label>", responseAsStringOrder);
        Assert.Contains("<label for=\"PhoneNumber\">PhoneNumber</label>", responseAsStringOrder);
        Assert.Contains("<label for=\"Email\">Email</label>", responseAsStringOrder);
        Assert.Contains("<label for=\"Adress\">Adress</label>", responseAsStringOrder);
        Assert.Contains("<label for=\"DeliveryOption\">DeliveryOption</label>", responseAsStringOrder);

        var model = new OrderFormModel
        {
          FirstName = "Ivan",
          LastName = "Petrov",
          DeliveryOption = Data.Models.Enums.DeliveryOption.Econt,
          Email = "ivan_petrovValid@abv.bg",
          Adress = "test adress streen number one",
          PhoneNumber = "0777777777"
        };

        var tokenMatch = Regex.Match(responseAsStringOrder, @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" />");
        var requestVerificationToken = tokenMatch.Success ? tokenMatch.Groups[1].Value : string.Empty;
        Assert.False(string.IsNullOrEmpty(requestVerificationToken), "CSRF token not found.");

        var content = new FormUrlEncodedContent(new[]
        {
           new KeyValuePair<string, string>("__RequestVerificationToken",requestVerificationToken),
           new KeyValuePair<string, string>("FirstName",model.FirstName),
           new KeyValuePair<string, string>("LastName",model.LastName),
           new KeyValuePair<string, string>("DeliveryOption",model.DeliveryOption.ToString()),
           new KeyValuePair<string, string>("Email",model.Email),
           new KeyValuePair<string, string>("Adress",model.Adress),
           new KeyValuePair<string, string>("PhoneNumber",model.PhoneNumber)
        });

        var postResponse = await userClient.PostAsync("/Order/CompleteOrder", content);

        postResponse.EnsureSuccessStatusCode();
        var postResponseAsString = await postResponse.Content.ReadAsStringAsync();

        Assert.True(postResponse.IsSuccessStatusCode);
        Assert.Contains("<script>toastr.success('You have successfully completed your order.Now we start processing it upon shipment');</script>", postResponseAsString);
        Assert.Contains("<td><strong>Name:</strong></td>", postResponseAsString);
    }

    [Fact]
    public async Task OrdersForProduct_Get_ShouldRedirectToSellerBecome_WhenUserDoesNotASeller()
    {
        var response = await userClient.GetAsync("/Order/OrdersForProduct");

        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("<script>toastr.error('You must be a become a seller to access this feature!');</script>", responseAsString);
        Assert.Contains("<h2 class=\"text-center\">Become Seller</h2>", responseAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"FirstName\">First Name:</label>", responseAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"LastName\">Last Name:</label>", responseAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"Description\">Description:</label>", responseAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"PhoneNumber\">Phone Number:</label>", responseAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"DateOfBirth\">Date of Birth:</label>", responseAsString);
        Assert.Contains("<label class=\"font-weight-bold\" for=\"Egn\">EGN:</label>", responseAsString);
    }

    [Fact]
    public async Task OrdersForProduct_Get_ShouldReturnViewError401_WhenUserIsSellerIsNotApproved()
    {
       var response = await notApprovedSellerClient.GetAsync("/Order/OrdersForProduct");

        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

        Assert.Contains("<script>toastr.error('Failed to retrieve the seller ID. Please try again or contact administrator!');</script>", responseAsString);
        Assert.Contains("<p>Oops, you don't have access here.</p>",responseAsString);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task OrdersForProduct_Get_ShouldReturnView_WhenSellerExists()
    {
        var response = await sellerClient.GetAsync("/Order/OrdersForProduct");

        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("<h2 class=\"text-center\">Orders for Delivering</h2>", responseAsString);
    }

    [Fact]
    public async Task SendOrder_Post_ShouldSendOrderSuccessfully_WhenOrderExists()
    {
        var response = await sellerClient.GetAsync("/Order/OrdersForProduct");
        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

        var tokenMatch = Regex.Match(responseAsString, @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" />");
        var requestVerificationToken = tokenMatch.Success ? tokenMatch.Groups[1].Value : string.Empty;
        Assert.False(string.IsNullOrEmpty(requestVerificationToken), "CSRF token not found.");

        var orderId = DataBaseSeeder.Order!.Id;

        var postData = new Dictionary<string, string>
        {
            { "__RequestVerificationToken", requestVerificationToken },
            { "OrderId", orderId.ToString() }
        };

        var content = new FormUrlEncodedContent(postData);

        var postResponse = await sellerClient.PostAsync("/Order/SendOrder", content);
        postResponse.EnsureSuccessStatusCode(); 

        var postResponseAsString = await postResponse.Content.ReadAsStringAsync();
        Assert.Contains("<script>toastr.success('You have successfully send order!');</script>", postResponseAsString);
    }

    private HttpClient BuildAuthenticateNotExistingUserClient()
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication("TestAuthentication")
                        .AddScheme<AuthenticationSchemeOptions, NotExistingUserAuthenticationHandler>("TestAuthentication", options => { });
            });
        }).CreateClient();
    }

    private HttpClient BuildAuthenticatedSellerClient()
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(service =>
            {
                service.AddAuthentication("TestAuthentication")
                       .AddScheme<AuthenticationSchemeOptions, SellerAuthenticationHandler>("TestAuthentication", null);
            });
        }).CreateClient();
    }

    private HttpClient BuildAuthenticateNotApprovedSellerClinet()
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication("TestAuthentication")
                        .AddScheme<AuthenticationSchemeOptions, NotApprovedSellerAuthenticationHandler>("TestAuthentication", options => { });
            });
        }).CreateClient();
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
}
