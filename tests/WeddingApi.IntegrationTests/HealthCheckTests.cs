using System.Net;

namespace WeddingApi.IntegrationTests;

public class HealthCheckTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;

    public HealthCheckTests()
    {
        _baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? throw new InvalidOperationException("API_BASE_URL environment variable must be set");
        _client = new HttpClient();
    }

    [Fact]
    public async Task HealthCheck()
    {
        var response = await _client.GetAsync($"{_baseUrl}/health");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine(content);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
} 