using IMDB.Application.Models;
using IMDB.Application.Repositories;

namespace IMDB.Application.Services;

public class MovieServices : IMovieServices
{
    private readonly IMovieRepository _movieRepository;

    public MovieServices(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public Task<bool> CreateAsync(Movie movie)
    {
        return _movieRepository.CreateAsync(movie);
    }

    public Task<Movie?> GetByIDAsync(Guid Id)
    {
        return _movieRepository.GetByIDAsync(Id);
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        return _movieRepository.GetBySlugAsync(slug);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return _movieRepository.GetAllAsync();
    }

    public async Task<Movie> UpdateAsync(Movie movie)
    {
        var movieExist = await _movieRepository.ExistByIdAsync(movie.Id);
        if (!movieExist)
        {
            return null;
        }

        await _movieRepository.UpdateAsync(movie);
        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid Id)
    {
        return _movieRepository.DeleteByIdAsync(Id);
    }
}