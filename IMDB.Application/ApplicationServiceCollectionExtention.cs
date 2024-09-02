using IMDB.Application.DataBase;
using IMDB.Application.Repositories;
using IMDB.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IMDB.Application;

public static class ApplicationServiceCollectionExtention
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IMovieRepository, MovieRepository>();
        services.AddSingleton<IMovieServices, MovieServices>();
        return services;
    }

    public static IServiceCollection AddDataBase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new SqlServerConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>();
        return services;
    }
}