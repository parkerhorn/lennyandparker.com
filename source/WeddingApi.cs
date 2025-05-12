using WeddingAPI.Models;
using WeddingAPI.Services.Interfaces;
using WeddingAPI.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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

        apiGroup.MapGet("health", async (ApplicationDbContext dbContext) => 
        {
            try
            {
                // Test the database connection
                await dbContext.Database.CanConnectAsync();
                
                return Results.Ok(new { 
                    Status = "Healthy", 
                    Timestamp = DateTime.UtcNow,
                    Components = new[] {
                        new { 
                            Name = "Database", 
                            Status = "Healthy",
                            Description = "SQL Server connection is working" 
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                var response = new { 
                    Status = "Unhealthy", 
                    Timestamp = DateTime.UtcNow,
                    Components = new[] {
                        new { 
                            Name = "Database", 
                            Status = "Unhealthy",
                            Description = $"SQL Server connection failed: {ex.Message}" 
                        }
                    }
                };
                
                return Results.Problem(
                    statusCode: 503,
                    title: "Health Check Failed",
                    detail: ex.Message,
                    instance: "/api/health");
            }
        })
            .WithName("HealthCheck")
            .WithDescription("Health check endpoint for testing database connectivity");

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

            existingInvitation.FirstName = invitation.FirstName;
            existingInvitation.LastName = invitation.LastName;
            existingInvitation.Email = invitation.Email;
            existingInvitation.IsAttending = invitation.IsAttending;
            existingInvitation.DietaryRestrictions = invitation.DietaryRestrictions;
            existingInvitation.AccessibilityRequirements = invitation.AccessibilityRequirements;
            existingInvitation.Pronouns = invitation.Pronouns;
            existingInvitation.UpdatedAt = DateTime.UtcNow;
            existingInvitation.RespondedAt = DateTime.UtcNow;

            var result = await service.UpdateAndSaveAsync(existingInvitation);
            return Results.Ok(result);
        })
        .WithName("UpdateRSVP")
        .WithDescription("Update an existing RSVP response");
    }
} 