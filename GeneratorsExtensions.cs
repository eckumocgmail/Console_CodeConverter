using DataConverter.Generators;

using Microsoft.Extensions.DependencyInjection;

public static class GeneratorsExtensions
{
    public static IServiceCollection AddDataConverters(this IServiceCollection services)
    {
        services.AddScoped<ModelConverter>();
        services.AddScoped<MyApplicationModelController>();
        return services;
    }

    public static IServiceCollection AddCodeGenerators(this IServiceCollection services)
    {
        services.AddScoped<ControllerGenerator>();
        services.AddScoped<DbcontextGenerator>();
        services.AddScoped<ModelGenerator>();
        services.AddScoped<RepositoryGenerator>();
        services.AddScoped<TableGenerator>();
        services.AddScoped<WebapiGenerator>();
        return services;
    }
}