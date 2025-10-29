using Microsoft.Extensions.DependencyInjection;

namespace TheCodeKitchen.Application.Business;

public static class ApplicationServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        //AutoMapper
        services.AddAutoMapper(typeof(ApplicationServiceRegistration));
    }
}