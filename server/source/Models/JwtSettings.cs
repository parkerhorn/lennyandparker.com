namespace WeddingApi.Models;

public class JwtSettings
{
  public string SecretKey { get; set; } = string.Empty;
  public string Issuer { get; set; } = string.Empty;
  public string Audience { get; set; } = string.Empty;
  public int ExpiryHours { get; set; } = 24;
}
