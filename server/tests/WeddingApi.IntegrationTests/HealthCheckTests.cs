using System.Net;
using System.Text.Json;
using Xunit.Abstractions;

namespace WeddingApi.IntegrationTests;

public class HealthCheckTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;
    private readonly ITestOutputHelper _output;

    public HealthCheckTests(ITestOutputHelper output)
    {
        _output = output;
        _baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? throw new InvalidOperationException("API_BASE_URL environment variable must be set");
        _client = new HttpClient();
    }

    [Fact]
    public async Task HealthCheck()
    {
        var response = await _client.GetAsync($"{_baseUrl}/health");

        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        
        _output.WriteLine(PrettyPrintJson(content));
    }
    
    private string PrettyPrintJson(string json)
    {
        try
        {
            var jsonElement = JsonDocument.Parse(json).RootElement;

            return JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            return json;
        }
    }

    public void Dispose()
    {
        _client.Dispose();
    }
} 