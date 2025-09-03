namespace WeddingApi.Models;

public class RSVP : GenericBaseClass
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAttending { get; set; }
    public string? DietaryRestrictions { get; set; }
    public string? AccessibilityRequirements { get; set; }
    public string? Pronouns { get; set; }
    public string? Note { get; set; }
    public Guid? PlusOneId { get; set; }
} 