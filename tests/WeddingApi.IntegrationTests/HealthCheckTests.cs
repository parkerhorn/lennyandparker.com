using System.Net;

namespace WeddingApi.IntegrationTests;

public class HealthCheckTests
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;

    public HealthCheckTests()
    {
        _baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL");
        _client = new HttpClient();
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync($"{_baseUrl}/health");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthyStatus()
    {
        // Act
        var response = await _client.GetAsync($"{_baseUrl}/health");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("Healthy", content);
    }

    [Fact]
    public async Task HealthCheck_DatabaseComponentIsHealthy()
    {
        // Act
        var response = await _client.GetAsync($"{_baseUrl}/health");
        var healthStatus = await response.Content.ReadFromJsonAsync<HealthStatus>();

        // Assert
        Assert.NotNull(healthStatus);
        Assert.Equal("Healthy", healthStatus.Status);
        
        var dbComponent = healthStatus.Components?.FirstOrDefault(c => c.Name == "Database");
        Assert.NotNull(dbComponent);
        Assert.Equal("Healthy", dbComponent.Status);
    }
}

public class HealthStatus
{
    public string Status { get; set; }
    public DateTime Timestamp { get; set; }
    public List<HealthComponent> Components { get; set; }
}

public class HealthComponent
{
    public string Name { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
} 