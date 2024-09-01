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

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into IMDB(Id , slug ,title, yearofrelease)
                                                                         values (@Id,@Slug,@Title,@YearOfRelease)
                                                                         """,movie));
        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                                                                    insert into Genres(movieId,name)
                                                                    values (@MovieId,@Name)
                                                                    """, new { MovieId = movie.Id, Name = genre }));
            }
        }
        transaction.Commit();
        return result > 0;
    }

    public async Task<Movie?> GetByIDAsync(Guid Id)
    {
        using var connection =await _dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                                  select * from IMDB where Id=@Id
                                  """, new { Id }));
        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("""
                                  select neme from genres where movieId=@Id
                                  """,new {Id}));
        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                                  select * from IMDB where Slug=@slug
                                  """, new { slug }));
        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("""
                                  select neme from genres where movieId=@Id
                                  """,new {Id = movie.Id}));
        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.QueryAsync(new CommandDefinition("""
                                                                       select m.*,string_agg(g.name,',')as genres
                                                                       from IMDB m left join genres g on m.Id=g.movieId
                                                                       group by Id
                                                                       """));

        return result.Select(x => new Movie
        {
            Id = x.Id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }

    public async Task<bool> UpdateAsync(Movie movie)
    {
        using var connction = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connction.BeginTransaction();
        await connction.ExecuteAsync(new CommandDefinition("""
                                                           delete from genres where movieId=@Id
                                                           """, new { Id = movie.Id }));
        foreach (var genre in movie.Genres)
        {
            await connction.ExecuteAsync(new CommandDefinition("""
                                                               insert into genres(movieId,name)
                                                               values (@MovieId,@Name)
                                                               """, new { MovieId = movie.Id, Name = genre }));
        }

        var result = await connction.ExecuteAsync(new CommandDefinition("""
                                                                        update IMDB set slug = @Slug,title=@Title,yearofrelease=@YearOfRelease
                                                                        where Id=@Id
                                                                        """, movie));
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid Id)
    { 
        using var connction = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connction.BeginTransaction();

        await connction.ExecuteAsync(new CommandDefinition("""
                                                           delete from genres where movieId=@Id
                                                           """, new { Id }));
        var result = await connction.ExecuteAsync(new CommandDefinition("""
                                                                        delete from IMDB where Id=@Id
                                                                        """, new { Id }));
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistByIdAsync(Guid Id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                                                                               select count(1) from IMDB where Id=@Id
                                                                               """, new { Id }));
    }
}