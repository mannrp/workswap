using System.Net;
using System.Net.Http.Json;
using workswap.DTOs;
using Xunit;

namespace workswap.Tests;

public class AuthTests : IClassFixture<WorkswapTestFixture>
{
    private readonly WorkswapTestFixture _fixture;
    private readonly HttpClient _client;

    public AuthTests(WorkswapTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.EnsureDatabaseCreated();
        _client = _fixture.CreateClient();
    }

    [Fact]
    public async Task RegisterAndLogin_ReturnsToken()
    {
        // Arrange
        var email = $"test_{Guid.NewGuid()}@example.com";
        var registerRequest = new RegisterRequest(
            Email: email,
            Password: "Test@123456",
            FirstName: "Test",
            LastName: "User"
        );

        // Act: Register
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        // Act: Login
        var loginRequest = new LoginRequest(Email: email, Password: "Test@123456");
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(loginResult?.Token);
    }

    [Fact]
    public async Task GetMe_ReturnsUserInfo_WhenAuthenticated()
    {
        // Arrange
        var (token, _) = await _fixture.CreateAndLoginUserAsync(_client, $"me_{Guid.NewGuid()}@example.com");
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var userInfo = await response.Content.ReadFromJsonAsync<UserInfoResponse>();
        Assert.NotNull(userInfo);
    }
}
