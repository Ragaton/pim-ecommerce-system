using System;
using Npgsql;

namespace PIM.Services;

public class DatabaseConnection
{
    public static string ConnectionString =
        Environment.GetEnvironmentVariable("PIM_DB_CONNECTION_STRING") ?? "";

    public static NpgsqlConnection GetConnection()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            throw new InvalidOperationException(
                "PIM_DB_CONNECTION_STRING environment variable is not set.");
        }

        return new NpgsqlConnection(ConnectionString);
    }
}