using System.Net;
using System.Net.Http.Json;
using workswap.DTOs;
using workswap.Models;
using Microsoft.Extensions.DependencyInjection;
using workswap.Data;
using Xunit;

namespace workswap.Tests;

public class ShiftTests : IClassFixture<WorkswapTestFixture>
{
    private readonly WorkswapTestFixture _fixture;
    private readonly HttpClient _client;

    public ShiftTests(WorkswapTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.EnsureDatabaseCreated();
        _client = _fixture.CreateClient();

        // Seed a test department
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (!db.Departments.Any())
        {
            db.Departments.Add(new Department { Name = "Test Department", Description = "Test" });
            db.SaveChanges();
        }
    }

    [Fact]
    public async Task CreateAndGetShift_ReturnsCreatedShift()
    {
        // Arrange
        var (token, userId) = await _fixture.CreateAndLoginUserAsync(_client, $"shifttest_{Guid.NewGuid()}@example.com");
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createShiftRequest = new CreateShiftRequest(
            Date: DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            StartTime: new TimeOnly(9, 0),
            EndTime: new TimeOnly(17, 0),
            DepartmentId: 1,
            AssignedUserId: userId,
            Notes: "Test shift",
            IsAvailableForSwap: true
        );

        // Act
        var createResponse = await _client.PostAsJsonAsync("/api/shifts", createShiftRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        
        var createdShift = await createResponse.Content.ReadFromJsonAsync<ShiftResponse>();
        Assert.NotNull(createdShift);

        var getResponse = await _client.GetAsync($"/api/shifts/{createdShift.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var retrievedShift = await getResponse.Content.ReadFromJsonAsync<ShiftResponse>();
        Assert.Equal(createdShift.Id, retrievedShift?.Id);
    }
}
