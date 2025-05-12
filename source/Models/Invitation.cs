namespace WeddingAPI.Models;

public class Invitation : GenericBaseClass
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAttending { get; set; }
    public string? DietaryRestrictions { get; set; }
    public string? AccessibilityRequirements { get; set; }
    public string[]? Pronouns { get; set; }
    public DateTime? RespondedAt { get; set; }

} 