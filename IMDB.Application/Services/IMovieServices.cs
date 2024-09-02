using IMDB.Application.Models;

namespace IMDB.Application.Services;

public interface IMovieServices
{
    Task<bool> CreateAsync(Movie movie);
    Task<Movie?> GetByIDAsync(Guid Id);
    Task<Movie?> GetBySlugAsync(string slug);
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<Movie> UpdateAsync(Movie movie);
    Task<bool> DeleteByIdAsync(Guid Id);
}