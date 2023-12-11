using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POS.Infrastructure.AppContext;

namespace POS.Infrastructure;

public static class RegisterService
{
    public static IServiceCollection InfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(x =>
            x.UseSqlServer(configuration.GetConnectionString("DbConn")));
        return services;
    }
}