using OnlineStore.Tests.IntegrationTesting;

namespace OnlineStore.Tests.AdminIntegrationTesting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OnlineStore.Data;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Tests.TestAuthentications;
using OnlineStore.Web.ViewModels.Admin;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using static DataBaseSeeder;

public class UserIntegrationTests : IClassFixture<IntegrationTestFactory<Program>>
{
    private readonly IntegrationTestFactory<Program> factory;
    private readonly HttpClient adminClien;
    private OnlineStoreDbContext _dbContext;
    private readonly Mock<IUserService> mockUserService;    

    public UserIntegrationTests(IntegrationTestFactory<Program> _factory)
    {
        mockUserService = new Mock<IUserService>();

        factory = _factory;
        adminClien = BuildAuthenticatedAdminClient();

        using (var scope = _factory.Services.CreateScope())
        {
            _dbContext = scope.ServiceProvider.GetRequiredService<OnlineStoreDbContext>();
            DataBaseSeeder.SeedDataBase(_dbContext);
        }
    }

    [Fact]
    public async Task All_Get_ShouldReturnInformationAboutUsers()
    {
        var response = await adminClien.GetAsync("/User/All");

        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.Contains("<h2 class=\"text-center\">All users</h2>", responseAsString);
        Assert.Contains("<th>Email</th>", responseAsString);
        Assert.Contains("<th>Full Name</th>", responseAsString);
        Assert.Contains("<th>Phone Number</th>", responseAsString);
        Assert.Contains("<th>Is Seller</th>", responseAsString);
        Assert.Contains("<th>Online Status</th>", responseAsString);
        Assert.Contains("<th>Actions</th>", responseAsString);
    }

    [Fact]
    public async Task BlockUser_Get_ShouldReturnViewToBlockUser_WhenUserExists()
    {
        var userIdToBlock = User!.Id.ToString();
        var response = await adminClien.GetAsync($"/Admin/User/BlockUser?userId={userIdToBlock}");
        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.Contains("<h4 class=\"text-danger\">If you block a user, they will never again be able to enter this site with their email.</h4>", responseAsString);
        Assert.Contains("<th>ID</th>", responseAsString);
        Assert.Contains("<th>Email</th>", responseAsString);
        Assert.Contains("<th>Full Name</th>", responseAsString);
        Assert.Contains("<th>Action</th>", responseAsString);
    }

    [Fact]
    public async Task BlockUser_Get_ShouldReturnError404_WhenUserDoesNotExists()
    {
        var notExistingUserId = Guid.NewGuid().ToString();
        var response = await adminClien.GetAsync($"/Admin/User/BlockUser?userId={notExistingUserId}");
        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.Contains("<script>toastr.error('The specified user account could not be found. Please try again later or contact with administrator!');</script>", responseAsString);
    }

    [Fact]
    public async Task BlockUser_Post_ShouldBlockUserSuccessfully_WhenUserExists()
    {
        var userIdToBlock = User!.Id.ToString();
        var response = await adminClien.GetAsync($"/Admin/User/BlockUser?userId={userIdToBlock}");
        response.EnsureSuccessStatusCode();

        var responseAsString = await response.Content.ReadAsStringAsync();

        var tokenMatch = Regex.Match(responseAsString, @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" />");
        var requestVerificationToken = tokenMatch.Success ? tokenMatch.Groups[1].Value : string.Empty;

        Assert.False(string.IsNullOrEmpty(requestVerificationToken), "CSRF token not found.");

        var postData = new Dictionary<string, string>
        {
            { "__RequestVerificationToken", requestVerificationToken },
            { "UserId", userIdToBlock.ToString() }
        };

        var content = new FormUrlEncodedContent(postData);

        var postResponse = await adminClien.PostAsync($"/Admin/User/BlockUser?userId={userIdToBlock}", content);
        postResponse.EnsureSuccessStatusCode();

        var postResponseAsString = await postResponse.Content.ReadAsStringAsync();
        Console.WriteLine(postResponseAsString);

        Assert.True(response.IsSuccessStatusCode);
        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
    }
    private HttpClient BuildAuthenticatedAdminClient()
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(service =>
            {
                service.AddAuthentication("TestAuthentication")
                       .AddScheme<AuthenticationSchemeOptions, AdminAuthenticationHandler>("TestAuthentication", null);
            });
        }).CreateClient();
    }
}
