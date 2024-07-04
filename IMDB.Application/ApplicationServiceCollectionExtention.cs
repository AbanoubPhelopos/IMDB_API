using IMDB.Application.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace IMDB.Application;

public static class ApplicationServiceCollectionExtention
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IMovieRepository, MovieRepository>();
        return services;
    }
}