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
                                      CREATE TABLE IF NOT EXISTS IMDB(
                                          Id UUID PRIMARY KEY,
                                          slug TEXT NOT NULL,
                                          title TEXT NOT NULL,
                                          yearofrelease INTEGER NOT NULL
                                      )
                                      """);

        await connection.ExecuteAsync("""
                                      create unique index concurrently if not exists IMDB_slug_idx
                                      on IMDB
                                      using btree(slug)
                                      """);
        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS Genres(
                                          movieId uuid references IMDB(Id),
                                          name text not null
                                      )
                                      """);
    }
    

}