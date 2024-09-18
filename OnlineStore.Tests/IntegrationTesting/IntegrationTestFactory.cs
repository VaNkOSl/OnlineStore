namespace OnlineStore.Tests.IntegrationTesting;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Data;
public class IntegrationTestFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public OnlineStoreDbContext DbContext { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                context.HostingEnvironment.EnvironmentName = "Testing";
            });

            services.AddDbContext<OnlineStoreDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                DbContext = scope.ServiceProvider.GetRequiredService<OnlineStoreDbContext>();
                DbContext.Database.EnsureCreated();
            }
        });
    }
}
