using Microsoft.AspNetCore.Mvc;
using WeddingApi.Models;
using WeddingApi.Repository;
using WeddingApi.Repository.Interfaces;
using WeddingApi.Services.Interfaces;

namespace WeddingApi.Helpers;

public class RSVPEndpointMapper : IEndpointMapper
{
  public void MapEndpoints(WebApplication app)
  {
    var rsvp = app.MapGroup("/rsvp").WithTags("RSVP").WithOpenApi(); // .RequireAuthorization(); // Commented out for testing

    rsvp.MapPost("/", async ([FromBody] IEnumerable<RSVP> rsvps, IGenericAsyncDataService<RSVP, ApplicationDbContext> service, IUnitOfWork<ApplicationDbContext> unitOfWork, ILogger<RSVPEndpointMapper> logger) =>
    {
      
      foreach (var r in rsvps)
      {
        await service.AddAsync(r);
      }

      await unitOfWork.SaveChangesAsync(new CancellationToken());

      var rsvpSummary = string.Join("; ", rsvps.Select(r => $"{r.FirstName} {r.LastName}: {(r.IsAttending ? "ATTENDING" : "NOT ATTENDING")}"));

      logger.LogInformation("Data: {RSVPSummary}", rsvpSummary);
      
      logger.LogInformation("RSVP submission completed successfully for {Count} attendees", rsvps.Count());
      
      return Results.Created("/rsvp", rsvps);
    });

    rsvp.MapGet("/", async (IGenericAsyncDataService<RSVP, ApplicationDbContext> service) =>
    {
      var result = await service.GetAllAsync();

      return result is null ? Results.NotFound() : Results.Ok(result);
    });

    rsvp.MapGet("/{id:guid}", async (Guid id, IGenericAsyncDataService<RSVP, ApplicationDbContext> service) =>
    {
      var result = await service.GetByIdAsync(id);

      return result is null ? Results.NotFound() : Results.Ok(result);
    });

    rsvp.MapPut("/{id:guid}", async (Guid id, RSVP rsvp, IGenericAsyncDataService<RSVP, ApplicationDbContext> service) =>
    {
      var item = await service.GetByIdAsync(id);

      if (item is null)
      {
        return Results.NotFound();
      }

      item.FirstName = rsvp.FirstName;
      item.LastName = rsvp.LastName;
      item.Email = rsvp.Email;
      item.IsAttending = rsvp.IsAttending;
      item.DietaryRestrictions = rsvp.DietaryRestrictions;
      item.AccessibilityRequirements = rsvp.AccessibilityRequirements;
      item.Pronouns = rsvp.Pronouns;
      item.Note = rsvp.Note;
      item.UpdatedAt = DateTime.UtcNow;

      return Results.Ok(await service.UpdateAndSaveAsync(item));
    });

    rsvp.MapDelete("/{id:guid}", async (Guid id, IGenericAsyncDataService<RSVP, ApplicationDbContext> service) =>
    {
      var item = await service.GetByIdAsync(id);

      if (item is null)
      {
        return Results.NotFound();
      }

      await service.DeleteAndSaveAsync(item);

      return Results.NoContent();
    });

    rsvp.MapGet("/search", async (
      [FromQuery] string? firstName,
      [FromQuery] string? lastName,
      IGenericAsyncDataService<RSVP, ApplicationDbContext> service,
      IFuzzyMatchService fuzzyService) =>
    {
      if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
      {
        return Results.BadRequest("At least firstName or lastName must be provided");
      }

      var allRsvps = await service.GetAllAsync();

      if (allRsvps == null || !allRsvps.Any())
      {
        return Results.NotFound("No RSVPs found");
      }

      var bestMatch = fuzzyService.FindBestMatch(allRsvps, firstName, lastName);

      if (bestMatch == null)
      {
        return Results.NotFound("No matching RSVP found");
      }

      return Results.Ok(bestMatch);
    })
    .WithName("SearchRSVPs")
    .WithDescription("Find the best matching RSVP by first and last name using fuzzy search (90%+ similarity)");
  }
}
