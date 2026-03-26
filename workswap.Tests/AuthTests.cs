using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using workswap.Data;
using workswap.DTOs;
using Xunit;

namespace workswap.Tests;

/// <summary>
/// Integration tests for the authentication flow.
/// Tests register → login → access protected endpoint.
/// </summary>
public class AuthTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly SqliteConnection _connection;

    public AuthTests(WebApplicationFactory<Program> factory)
    {
        // A single shared connection keeps the in-memory database alive
        // for the entire lifetime of the test class.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            // Force "Testing" environment so migration/seed is skipped.
            builder.UseEnvironment("Testing");

            // Provide JWT config that's normally in appsettings.Development.json
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
                // Strip every EF registration that Program.cs added for SQLite
                var descriptorsToRemove = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
                             || d.ServiceType == typeof(ApplicationDbContext))
                    .ToList();
                foreach (var d in descriptorsToRemove)
                    services.Remove(d);

                // Re-register the DbContext using the SHARED connection
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });
            });
        });

        // Ensure schema is created via the app's own service provider
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();

        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    [Fact]
    public async Task RegisterLoginAndAccessMe_ReturnsUserInfo()
    {
        // Arrange
        var registerRequest = new RegisterRequest(
            Email: "test@example.com",
            Password: "Test@123456",
            FirstName: "Test",
            LastName: "User"
        );

        // Act 1: Register
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert 1: Registration successful
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(registerResult);
        Assert.True(registerResult.Success);
        Assert.NotNull(registerResult.Token);

        // Act 2: Login
        var loginRequest = new LoginRequest(
            Email: "test@example.com",
            Password: "Test@123456"
        );
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert 2: Login successful
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(loginResult);
        Assert.True(loginResult.Success);
        Assert.NotNull(loginResult.Token);

        // Act 3: Access protected endpoint
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
        var meResponse = await _client.GetAsync("/api/auth/me");

        // Assert 3: Protected endpoint accessible
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        var userInfo = await meResponse.Content.ReadFromJsonAsync<UserInfoResponse>();
        Assert.NotNull(userInfo);
        Assert.Equal("test@example.com", userInfo.Email);
        Assert.Equal("Test", userInfo.FirstName);
        Assert.Equal("User", userInfo.LastName);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest(
            Email: "nonexistent@example.com",
            Password: "WrongPassword123"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert — API returns 401 Unauthorized for invalid credentials
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
