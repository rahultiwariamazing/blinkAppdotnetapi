using BlinkDemoApi.Application.Auth.Interfaces;
using BlinkDemoApi.Application.Auth.Services;
using BlinkDemoApi.Application.Catalog.Services;
using BlinkDemoApi.Application.Me.Services;
using BlinkDemoApi.Application.Addresses.Services;
using BlinkDemoApi.Application.Cart.Services;
using BlinkDemoApi.Application.Orders.Services;
using BlinkDemoApi.Infrastructure.Data;
using BlinkDemoApi.Infrastructure.Repositories;
using BlinkDemoApi.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlinkDemoApi.Infrastructure;

/// <summary>
/// Registers Infrastructure + Application services in one place.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var conn = config.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(conn))
            throw new InvalidOperationException("ConnectionStrings:Default is missing. Set it in appsettings.Development.json for local dev or via environment variable ConnectionStrings__Default.");

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(conn));

        // Repositories
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IMeRepository, MeRepository>();

        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        // Security
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        services.AddOptions<JwtOptions>()
                .Bind(config.GetSection("Jwt"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        // Application services
        services.AddScoped<AuthService>();
        services.AddScoped<MeService>();

        services.AddScoped<CatalogService>();
        services.AddScoped<AddressService>();
        services.AddScoped<CartService>();
        services.AddScoped<OrderService>();

        return services;
    }
}
