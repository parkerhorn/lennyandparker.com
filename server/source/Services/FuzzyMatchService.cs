using FuzzySharp;
using WeddingApi.Models;
using WeddingApi.Services.Interfaces;

namespace WeddingApi.Services;

public class FuzzyMatchService : IFuzzyMatchService
{
  private const int MINIMUM_THRESHOLD = 90;
  private const int SEARCH_LIMIT = 10;

  public RSVP? FindBestMatch(IEnumerable<RSVP> rsvps, string? firstName, string? lastName)
  {
    if (!rsvps.Any())
      return null;

    if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
      return null;

    var bestMatch = rsvps
        .Select(rsvp => new { RSVP = rsvp, Score = CalculateMatchScore(firstName, lastName, rsvp) })
        .Where(result => result.Score >= MINIMUM_THRESHOLD)
        .OrderByDescending(result => result.Score)
        .Take(SEARCH_LIMIT)
        .FirstOrDefault();

    return bestMatch?.RSVP;
  }

  private int CalculateMatchScore(string? firstName, string? lastName, RSVP rsvp)
  {
    var scores = new List<int>();

    var searchFirst = firstName?.Trim() ?? "";
    var searchLast = lastName?.Trim() ?? "";
    var rsvpFirst = rsvp.FirstName?.Trim() ?? "";
    var rsvpLast = rsvp.LastName?.Trim() ?? "";

    if (!string.IsNullOrEmpty(searchFirst) && !string.IsNullOrEmpty(searchLast))
    {
      var searchFullName = $"{searchFirst} {searchLast}";
      var rsvpFullName = $"{rsvpFirst} {rsvpLast}";

      scores.Add(Fuzz.Ratio(searchFullName, rsvpFullName));
      scores.Add(Fuzz.PartialRatio(searchFullName, rsvpFullName));
      scores.Add(Fuzz.TokenSortRatio(searchFullName, rsvpFullName));
      scores.Add(Fuzz.TokenSetRatio(searchFullName, rsvpFullName));
    }

    if (!string.IsNullOrEmpty(searchFirst))
    {
      scores.Add(Fuzz.Ratio(searchFirst, rsvpFirst));
      scores.Add(Fuzz.PartialRatio(searchFirst, rsvpFirst));
    }

    if (!string.IsNullOrEmpty(searchLast))
    {
      scores.Add(Fuzz.Ratio(searchLast, rsvpLast));
      scores.Add(Fuzz.PartialRatio(searchLast, rsvpLast));
    }

    if (!string.IsNullOrEmpty(searchFirst) && !string.IsNullOrEmpty(searchLast))
    {
      scores.Add(Fuzz.Ratio(searchFirst, rsvpLast));
      scores.Add(Fuzz.Ratio(searchLast, rsvpFirst));
    }

    return scores.Any() ? scores.Max() : 0;
  }
}
