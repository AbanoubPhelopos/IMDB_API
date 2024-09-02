using Dapper;
using IMDB.Application.DataBase;
using IMDB.Application.Models;

namespace IMDB.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var result = await connection.ExecuteAsync(new CommandDefinition("""
                INSERT INTO Movies(Id, slug, title, yearofrelease)
                VALUES (@Id, @Slug, @Title, @YearOfRelease)
            """, movie, transaction));

            if (result > 0)
            {
                foreach (var genre in movie.Genres)
                {
                    await connection.ExecuteAsync(new CommandDefinition("""
                        INSERT INTO Genres(movieId, name)
                        VALUES (@MovieId, @Name)
                    """, new { MovieId = movie.Id, Name = genre }, transaction));
                }

                transaction.Commit();
                return true;
            }

            transaction.Rollback();
            return false;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<Movie?> GetByIDAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            SELECT * FROM Movies WHERE Id = @Id
        """, new { Id = id }));

        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
            SELECT name FROM Genres WHERE movieId = @Id
        """, new { Id = id }));

        movie.Genres = genres.ToList();
        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            SELECT * FROM Movies WHERE slug = @Slug
        """, new { Slug = slug }));

        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
            SELECT name FROM Genres WHERE movieId = @Id
        """, new { Id = movie.Id }));

        movie.Genres = genres.ToList();
        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        
        var result = await connection.QueryAsync(new CommandDefinition("""
                                                                       select m.*,string_agg(g.name,',')as genres
                                                                       from Movies m 
                                                                        left join genres g on m.Id=g.movieId
                                                                       group by Id
                                                                       """));

        return result.Select(x => new Movie
        {
            Id = x.Id != null ? (Guid)x.Id : Guid.Empty, 
            Title = x.title ?? string.Empty,         
            YearOfRelease = x.yearofrelease ?? 0,      
            Genres = x.genres as IEnumerable<string> != null 
                ? ((IEnumerable<string>)x.genres).ToList() 
                : new List<string>()
        });

    }

    public async Task<bool> UpdateAsync(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Delete existing genres
            await connection.ExecuteAsync(new CommandDefinition("""
                DELETE FROM Genres WHERE movieId = @Id
            """, new { Id = movie.Id }, transaction));

            // Insert new genres
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    INSERT INTO Genres(movieId, name)
                    VALUES (@MovieId, @Name)
                """, new { MovieId = movie.Id, Name = genre }, transaction));
            }

            var result = await connection.ExecuteAsync(new CommandDefinition("""
                UPDATE Movies 
                SET slug = @Slug, title = @Title, yearofrelease = @YearOfRelease
                WHERE Id = @Id
            """, movie, transaction));

            transaction.Commit();
            return result > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Delete genres first
            await connection.ExecuteAsync(new CommandDefinition("""
                DELETE FROM Genres WHERE movieId = @Id
            """, new { Id = id }, transaction));

            // Delete the movie itself
            var result = await connection.ExecuteAsync(new CommandDefinition("""
                DELETE FROM Movies WHERE Id = @Id
            """, new { Id = id }, transaction));

            transaction.Commit();
            return result > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> ExistByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
            SELECT COUNT(1) FROM Movies WHERE Id = @Id
        """, new { Id = id }));
    }
}