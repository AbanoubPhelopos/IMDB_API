using Dapper;

namespace IMDB.Application.DataBase;

public class DbInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync("""
                                          CREATE TABLE IF NOT EXISTS Movies(
                                              Id UUID PRIMARY KEY NOT NULL,
                                              slug TEXT NOT NULL,
                                              title TEXT NOT NULL,
                                              yearofrelease INTEGER NOT NULL
                                          )
                                      """);

        await connection.ExecuteAsync("""
                                          CREATE UNIQUE INDEX IF NOT EXISTS Movies_slug_idx
                                          ON Movies (slug)
                                      """);

        await connection.ExecuteAsync("""
                                          CREATE TABLE IF NOT EXISTS Genres (
                                              movieId UUID NOT NULL REFERENCES Movies(Id),
                                              name TEXT NOT NULL
                                          )
                                      """);

    }
}