namespace OnlineStore;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Extensions;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.Infrastructure.Extensions;
using static OnlineStore.Commons.GeneralApplicationConstants;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //if (builder.Environment.EnvironmentName == "Development")
        //{
        //    builder.Services.AddDbContext<OnlineStoreDbContext>(options =>
        //        options.UseInMemoryDatabase("InMemoryDbForTesting"));
        //}

        builder.Services.AddApplicationDbContext(builder.Configuration);
        builder.Services.AddApplicationIdentity(builder.Configuration);
        builder.Services.AddApplicationServices(typeof(IProductService));
        builder.Services.AddHostedService<OrderStatusUpdater>();

        builder.Services.AddRecaptchaService();

        builder.Services.AddMemoryCache();
        builder.Services.AddResponseCaching();

        builder.Services.ConfigureApplicationCookie(cfg =>
        {
            cfg.LoginPath = "/Identity/Account/Login";
            cfg.AccessDeniedPath = "/Home/Error/401";
        });

        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error/500");
            app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");

            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseResponseCaching();

        app.EnableOnlineUsersCheck();
        
        if (app.Environment.IsDevelopment())
        {
            app.SeedAdministrator(DevelopmentAdminEmail);
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(config =>
        {
            config.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
        );

            config.MapControllerRoute(
              name: "Product Details",
              pattern: "/Product/Details/{id}",
              defaults: new { Controller = "Product", Action = "Details" }
          );

            config.MapDefaultControllerRoute();

            config.MapRazorPages();

        });
        await app.RunAsync();
    }
}
