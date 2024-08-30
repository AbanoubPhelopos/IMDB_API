using System.Data;
using Npgsql;

namespace IMDB.Application.DataBase;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}

public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connctionString;

    public SqlServerConnectionFactory(string connctionString)
    {
        _connctionString = connctionString;
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connctionString);
        await connection.OpenAsync();
        return connection;
    }
}