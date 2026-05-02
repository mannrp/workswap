using System.Net;
using System.Net.Http.Json;
using workswap.DTOs;
using workswap.Models;
using Microsoft.Extensions.DependencyInjection;
using workswap.Data;
using Xunit;

namespace workswap.Tests;

public class SwapTests : IClassFixture<WorkswapTestFixture>
{
    private readonly WorkswapTestFixture _fixture;
    private readonly HttpClient _client;

    public SwapTests(WorkswapTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.EnsureDatabaseCreated();
        _client = _fixture.CreateClient();

        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (!db.Departments.Any())
        {
            db.Departments.Add(new Department { Name = "Test Dept", Description = "Test" });
            db.SaveChanges();
        }
    }

    [Fact]
    public async Task CreateSwapRequest_AndRespond_WorksCorrectly()
    {
        // 1. Setup two users
        var (token1, user1Id) = await _fixture.CreateAndLoginUserAsync(_client, $"sender_{Guid.NewGuid()}@example.com");
        var (token2, user2Id) = await _fixture.CreateAndLoginUserAsync(_client, $"receiver_{Guid.NewGuid()}@example.com");

        // 2. Create a shift for sender
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token1);
        var createShiftRequest = new CreateShiftRequest(
            Date: DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            StartTime: new TimeOnly(9, 0),
            EndTime: new TimeOnly(17, 0),
            DepartmentId: 1,
            AssignedUserId: user1Id,
            Notes: "Sender Shift"
        );
        var shiftRes = await _client.PostAsJsonAsync("/api/shifts", createShiftRequest);
        var senderShift = await shiftRes.Content.ReadFromJsonAsync<ShiftResponse>();

        // 3. Create swap request
        var createSwapDto = new CreateSwapDto(
            SenderShiftId: senderShift!.Id,
            ReceiverShiftId: null,
            ReceiverId: user2Id
        );
        var swapRes = await _client.PostAsJsonAsync("/api/swaps", createSwapDto);
        Assert.Equal(HttpStatusCode.Created, swapRes.StatusCode);
        var swap = await swapRes.Content.ReadFromJsonAsync<SwapRequestResponse>();

        // 4. Respond to swap as receiver
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token2);
        var responseDto = new SwapResponseDto(Accepted: true);
        var respondRes = await _client.PutAsJsonAsync($"/api/swaps/{swap!.Id}/respond", responseDto);
        Assert.Equal(HttpStatusCode.OK, respondRes.StatusCode);

        // 5. Verify shift transfer
        var getShiftRes = await _client.GetAsync($"/api/shifts/{senderShift.Id}");
        var updatedShift = await getShiftRes.Content.ReadFromJsonAsync<ShiftResponse>();
        Assert.Equal(user2Id, updatedShift?.AssignedUserId);
    }
}
