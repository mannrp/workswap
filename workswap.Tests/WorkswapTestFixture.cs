using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using workswap.Data;
using workswap.DTOs;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;

namespace workswap.Tests;

/// <summary>
/// A shared test fixture that provides an in-memory database and a configured HttpClient.
/// This eliminates duplication across test classes and ensures a clean slate for each test suite.
/// </summary>
public class WorkswapTestFixture : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((ctx, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWT_SECRET"] = "test-secret-key-minimum-32-characters-long-for-hmac-sha256",
                ["JWT_ISSUER"] = "workswap-test",
                ["JWT_AUDIENCE"] = "workswap-test-client",
                ["JWT_DURATION_MINUTES"] = "60"
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptorsToRemove = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
                         || d.ServiceType == typeof(ApplicationDbContext))
                .ToList();
            
            foreach (var d in descriptorsToRemove)
                services.Remove(d);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });
        });
    }

    public void EnsureDatabaseCreated()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();

        // Seed roles
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        if (!roleManager.RoleExistsAsync("Employee").Result)
        {
            roleManager.CreateAsync(new IdentityRole<int>("Employee")).Wait();
        }
        if (!roleManager.RoleExistsAsync("Manager").Result)
        {
            roleManager.CreateAsync(new IdentityRole<int>("Manager")).Wait();
        }
    }

    public async Task<(string Token, int UserId)> CreateAndLoginUserAsync(HttpClient client, string email = "test@example.com")
    {
        var registerRequest = new RegisterRequest(
            Email: email,
            Password: "Test@123456",
            FirstName: "Test",
            LastName: "User"
        );

        await client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest(Email: email, Password: "Test@123456");
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Get user ID from /api/auth/me
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);
        var meResponse = await client.GetAsync("/api/auth/me");
        var userInfo = await meResponse.Content.ReadFromJsonAsync<UserInfoResponse>();

        return (loginResult.Token, userInfo!.Id);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Close();
        _connection?.Dispose();
    }
}
