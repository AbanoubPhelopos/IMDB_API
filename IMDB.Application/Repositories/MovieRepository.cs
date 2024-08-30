using Dapper;
using IMDB.Application.DataBase;
using IMDB.Application.Models;

namespace IMDB.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly List<Movie> _movies = new();
    
    //private readonly IDbConnectionFactory _dbConnectionFactory;
    //public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    //{
        //_dbConnectionFactory = dbConnectionFactory;
    //}

    public Task<bool> CreateAsync(Movie movie)
    {
        _movies.Add(movie);
        return Task.FromResult(true);
    }

    public Task<Movie?> GetByIDAsync(Guid Id)
    {
        var movie = _movies.SingleOrDefault(x =>x.Id == Id);
        return Task.FromResult(movie);
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        var movie = _movies.SingleOrDefault(x =>x.Slug == slug);
        return Task.FromResult(movie);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return Task.FromResult(_movies.AsEnumerable());
    }

    public Task<bool> UpdateAsync(Movie movie)
    {
        var movieIndex = _movies.FindIndex(x => x.Id == movie.Id);
        if (movieIndex == -1)
        {
            return Task.FromResult(false);
        }

        _movies[movieIndex] = movie;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteByIdAsync(Guid Id)
    {
        var removeCount = _movies.RemoveAll(x => x.Id == Id);
        var movieRemoved = removeCount > 0;
        return Task.FromResult(movieRemoved);
    }
}