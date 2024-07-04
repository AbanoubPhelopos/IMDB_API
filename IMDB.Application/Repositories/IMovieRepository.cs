using IMDB.Application.Models;

namespace IMDB.Application.Repositories;

public interface IMovieRepository
{
    Task<bool> CreateAsync(Movie movie);
    Task<Movie?> GetByIDAsync(Guid Id);
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<bool> UpdateAsync(Movie movie);
    Task<bool> DeleteByIdAsync(Guid Id);
}