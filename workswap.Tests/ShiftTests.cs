using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using workswap.Data;
using workswap.DTOs;
using workswap.Models;
using Xunit;

namespace workswap.Tests;

/// <summary>
/// Integration tests for shift CRUD operations.
/// Tests creating and retrieving shifts through the API.
/// </summary>
public class ShiftTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly SqliteConnection _connection;

    public ShiftTests(WebApplicationFactory<Program> factory)
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _factory = factory.WithWebHostBuilder(builder =>
        {
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
        });

        // Seed through the app's real service provider
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();

        // Seed a test department
        if (!db.Departments.Any())
        {
            db.Departments.Add(new Department
            {
                Name = "Test Department",
                Description = "Test"
            });
            db.SaveChanges();
        }

        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    [Fact]
    public async Task CreateAndGetShift_ReturnsCreatedShift()
    {
        // Arrange: Register and login to get auth token
        var registerRequest = new RegisterRequest(
            Email: "shifttest@example.com",
            Password: "Test@123456",
            FirstName: "Shift",
            LastName: "Tester"
        );
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest(
            Email: "shifttest@example.com",
            Password: "Test@123456"
        );
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(loginResult?.Token);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);

        // Arrange: Create shift request
        var createShiftRequest = new CreateShiftRequest(
            Date: DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            StartTime: new TimeOnly(9, 0),
            EndTime: new TimeOnly(17, 0),
            DepartmentId: 1,
            AssignedUserId: null,
            Notes: "Test shift"
        );

        // Act 1: Create shift
        var createResponse = await _client.PostAsJsonAsync("/api/shifts", createShiftRequest);

        // Assert 1: Shift created successfully
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdShift = await createResponse.Content.ReadFromJsonAsync<ShiftResponse>();
        Assert.NotNull(createdShift);
        Assert.True(createdShift.Id > 0);
        Assert.Equal("Test shift", createdShift.Notes);
        Assert.Equal("Test Department", createdShift.DepartmentName);

        // Act 2: Get the created shift
        var getResponse = await _client.GetAsync($"/api/shifts/{createdShift.Id}");

        // Assert 2: Shift retrieved successfully
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var retrievedShift = await getResponse.Content.ReadFromJsonAsync<ShiftResponse>();
        Assert.NotNull(retrievedShift);
        Assert.Equal(createdShift.Id, retrievedShift.Id);
        Assert.Equal(createdShift.Notes, retrievedShift.Notes);
    }

    [Fact]
    public async Task GetAllShifts_WithFilters_ReturnsFilteredShifts()
    {
        // Arrange: Register and login
        var registerRequest = new RegisterRequest(
            Email: "filtertest@example.com",
            Password: "Test@123456",
            FirstName: "Filter",
            LastName: "Tester"
        );
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest(
            Email: "filtertest@example.com",
            Password: "Test@123456"
        );
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(loginResult?.Token);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);

        // Arrange: Create multiple shifts
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var shift1 = new CreateShiftRequest(tomorrow, new TimeOnly(9, 0), new TimeOnly(17, 0), 1, null, "Morning shift");
        var shift2 = new CreateShiftRequest(tomorrow, new TimeOnly(17, 0), new TimeOnly(23, 0), 1, null, "Evening shift");

        await _client.PostAsJsonAsync("/api/shifts", shift1);
        await _client.PostAsJsonAsync("/api/shifts", shift2);

        // Act: Get all shifts for department 1
        var response = await _client.GetAsync("/api/shifts?departmentId=1");

        // Assert: Both shifts returned
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var shifts = await response.Content.ReadFromJsonAsync<List<ShiftResponse>>();
        Assert.NotNull(shifts);
        Assert.True(shifts.Count >= 2);
    }
}
