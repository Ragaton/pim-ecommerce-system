using System;
namespace PIM.Tests;

/// <summary>
/// Central connection configuration for all integration tests.
/// Points to the dedicated Supabase test database — never the production instance.
/// </summary>

public static class TestDatabaseConnection
{
    public static string ConnectionString =>
        Environment.GetEnvironmentVariable("PIM_TEST_DB_CONNECTION_STRING")
        ?? throw new InvalidOperationException(
            "PIM_TEST_DB_CONNECTION_STRING environment variable is not set.");
}