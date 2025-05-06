using WeddingAPI.Models;
using WeddingAPI.Services.Interfaces;
using WeddingAPI.Repository;

namespace WeddingAPI;

public static class WeddingApi
{
    public static void MapEndpoints(WebApplication app)
    {
        var apiGroup = app.MapGroup("api")
            .WithTags("API")
            .WithOpenApi();

        // System endpoints
        apiGroup.MapGet("", () => "Lenny and Parker are getting married!")
            .WithName("Root")
            .WithDescription("Root endpoint for the API");

        apiGroup.MapGet("health", () => Results.Ok(new { Status = "Healthy" }))
            .WithName("HealthCheck")
            .WithDescription("Health check endpoint for Azure");

        // RSVP endpoints
        var rsvpGroup = apiGroup.MapGroup("rsvp")
            .WithTags("RSVP")
            .WithOpenApi();

        rsvpGroup.MapPost("", async (Invitation invitation, IGenericAsyncDataService<Invitation, ApplicationDbContext> service) =>
        {
            invitation.RespondedAt = DateTime.UtcNow;
            var result = await service.AddAndSaveAsync(invitation);
            return Results.Created($"/api/rsvp/{result.Id}", result);
        })
        .WithName("CreateRSVP")
        .WithDescription("Create a new RSVP response");

        rsvpGroup.MapGet("", async (IGenericAsyncDataService<Invitation, ApplicationDbContext> service) =>
        {
            var invitations = await service.GetAllAsync();
            return Results.Ok(invitations);
        })
        .WithName("GetAllRSVPs")
        .WithDescription("Get all RSVP responses");

        rsvpGroup.MapGet("{id}", async (Guid id, IGenericAsyncDataService<Invitation, ApplicationDbContext> service) =>
        {
            var invitation = await service.GetByIdAsync(id);
            return invitation is null ? Results.NotFound() : Results.Ok(invitation);
        })
        .WithName("GetRSVPById")
        .WithDescription("Get a specific RSVP response by ID");

        rsvpGroup.MapPut("{id}", async (Guid id, Invitation invitation, IGenericAsyncDataService<Invitation, ApplicationDbContext> service) =>
        {
            var existingInvitation = await service.GetByIdAsync(id);

            if (existingInvitation is null)
                return Results.NotFound();

            existingInvitation.Name = invitation.Name;
            existingInvitation.Email = invitation.Email;
            existingInvitation.IsAttending = invitation.IsAttending;
            existingInvitation.NumberOfAttendees = invitation.NumberOfAttendees;
            existingInvitation.DietaryRestrictions = invitation.DietaryRestrictions;
            existingInvitation.UpdatedAt = DateTime.UtcNow;
            existingInvitation.RespondedAt = DateTime.UtcNow;

            var result = await service.UpdateAndSaveAsync(existingInvitation);
            return Results.Ok(result);
        })
        .WithName("UpdateRSVP")
        .WithDescription("Update an existing RSVP response");
    }
} 