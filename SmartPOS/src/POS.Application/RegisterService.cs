using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POS.Application.Common;
using POS.Application.Profiles;
using POS.Application.Repository;
using static System.Net.Mime.MediaTypeNames;

namespace POS.Application;

public static class RegisterService
{
    public static IServiceCollection ApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(x => {
            x.AddMaps(typeof(IApplication).Assembly);

        });
        services.AddValidatorsFromAssembly(typeof(IApplication).Assembly);

        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        services.AddScoped(typeof(IBaseRepository<,,>), typeof(BaseRepository<,,>));
        //services.AddTransient<ICityRepository, CityRepository>();


        services.Scan(scan => scan.FromAssemblyOf<IApplication>()
            .AddClasses(classes => classes.AssignableTo<IApplication>())
            .AddClasses(x => x.AssignableTo(typeof(IBaseRepository<,,>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        return services;
    }
}