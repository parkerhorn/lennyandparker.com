using WeddingApi.Models;

namespace WeddingApi.Services.Interfaces;

public interface IFuzzyMatchService
{
  /// <summary>
  /// Finds the best matching RSVP by first and last name using fuzzy search
  /// Uses a high threshold (90+) to ensure quality matches
  /// </summary>
  /// <param name="rsvps">Collection of RSVPs to search</param>
  /// <param name="firstName">First name to search for (optional)</param>
  /// <param name="lastName">Last name to search for (optional)</param>
  /// <returns>Best matching RSVP or null if no good match found</returns>
  RSVP? FindBestMatch(IEnumerable<RSVP> rsvps, string? firstName, string? lastName);
}
