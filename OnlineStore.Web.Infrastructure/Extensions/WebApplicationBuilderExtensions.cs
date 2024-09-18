
namespace OnlineStore.Web.Infrastructure.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Data.Models;
using OnlineStore.Web.Infrastructure.Middlewares;
using static OnlineStore.Commons.GeneralApplicationConstants;

public static class WebApplicationBuilderExtensions
{
    public static IApplicationBuilder SeedAdministrator(this IApplicationBuilder app,string email)
    {
        using IServiceScope serviceScope = app.ApplicationServices.CreateScope();

        IServiceProvider serviceProvider = serviceScope.ServiceProvider;

        UserManager<ApplicationUser> userManager = 
            serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        RoleManager<IdentityRole<Guid>> roleManager =
            serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        Task.Run(async() =>
        {
            if(await roleManager.RoleExistsAsync(AdminRoleName))
            {
                return;
            }

            IdentityRole<Guid> role =
                  new IdentityRole<Guid>(AdminRoleName);

            await roleManager.CreateAsync(role);

            ApplicationUser adminUser = await
                          userManager.FindByEmailAsync(email);

            if(adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email
                };

                string defaultPassword = "Admin123";

                await userManager.CreateAsync(adminUser, defaultPassword);
            }

            await userManager.AddToRoleAsync(adminUser, AdminRoleName);
        })
        .GetAwaiter()
        .GetResult();

        return app;
    }

    public static IApplicationBuilder EnableOnlineUsersCheck(this IApplicationBuilder app)
    {
        return app.UseMiddleware<OnlineUsersMiddleware>();
    }
}
