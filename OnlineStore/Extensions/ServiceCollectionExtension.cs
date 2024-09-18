namespace Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Data.Data.Common;
using OnlineStore.Data.Models;
using System.Reflection;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// This method registers all services with their interfaces and implementations of given assembly.
    /// The assembly is taken from the type of random service interface or implementation provided.
    /// </summary>
    /// <param name="serviceType"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void AddApplicationServices(this IServiceCollection services, Type servicetype)
    {
        Assembly? serviceAssembly = Assembly.GetAssembly(servicetype);
        
        if(serviceAssembly == null)
        {
            throw new InvalidOperationException("Invalid service type provided!");
        }

        Type[] implementationTypes = serviceAssembly
            .GetTypes()
            .Where(t => t.Name.EndsWith("Service") && !t.IsInterface)
            .ToArray();

        foreach(Type implementationType in implementationTypes)
        {
            Type? interfaceType = implementationType
                .GetInterface($"I{implementationType.Name}");

            if (interfaceType == null)
            {
                throw new InvalidOperationException(
                    $"No interface is provided for the service with name: {implementationType.Name}");
            }

            services.AddScoped(interfaceType, implementationType);
        }
    }

    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        services.AddDbContext<OnlineStoreDbContext>(options =>
            options.UseSqlServer(connectionString));

         services.AddScoped<IRepository, Repository>();
        //services.AddScoped<IFileService, FileService>();
        services.AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }

    public static IServiceCollection AddApplicationIdentity(this IServiceCollection services, IConfiguration config)
    {
        services.AddDefaultIdentity<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<OnlineStoreDbContext>();

        return services;
    }
}
