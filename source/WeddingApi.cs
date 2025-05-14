using WeddingAPI.Models;
using WeddingAPI.Services.Interfaces;
using WeddingAPI.Repository;
using WeddingAPI.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WeddingAPI;

public static class WeddingApi
{
    public static void MapEndpoints(WebApplication app)
    {
        var rsvpEndpoints = app.MapGroup("")
            .WithTags("RSVP")
            .WithOpenApi();

        rsvpEndpoints.MapGet("/", () => "Lenny and Parker are getting married!")
            .WithName("Root")
            .WithDescription("Root endpoint for the API");

        rsvpEndpoints.MapGet("/health", async (ApplicationDbContext dbContext) =>
        {
            try
            {
                await dbContext.Database.CanConnectAsync();

                return Results.Ok(new
                {
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
                var response = new
                {
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
                    instance: "/health"); // Updated instance path
            }
        })
            .WithName("HealthCheck")
            .WithDescription("Health check endpoint for testing database connectivity");

        rsvpEndpoints.MapPost("/rsvp", async ([FromBody] IEnumerable<RSVP> rsvps, IGenericAsyncDataService<RSVP, ApplicationDbContext> service, IUnitOfWork<ApplicationDbContext> unitOfWork) =>
        {
            var rsvpsList = rsvps.ToList();

            foreach (var rsvp in rsvpsList)
            {
                await service.AddAsync(rsvp);
            }

            await unitOfWork.SaveChangesAsync(new CancellationToken());
            return Results.Created($"/rsvp", rsvpsList);
        })
        .WithName("CreateRSVPs")
        .WithDescription("Create multiple RSVP responses")
        .Produces(StatusCodes.Status201Created);

        rsvpEndpoints.MapGet("/rsvp", async (IGenericAsyncDataService<RSVP, ApplicationDbContext> service) =>
        {
            var rsvps = await service.GetAllAsync();
            return Results.Ok(rsvps);
        })
        .WithName("GetAllRSVPs")
        .WithDescription("Get all RSVP responses");

        rsvpEndpoints.MapGet("/rsvp/{id}", async (Guid id, IGenericAsyncDataService<RSVP, ApplicationDbContext> service) =>
        {
            var rsvp = await service.GetByIdAsync(id);
            return rsvp is null ? Results.NotFound() : Results.Ok(rsvp);
        })
        .WithName("GetRSVPById")
        .WithDescription("Get a specific RSVP response by ID");

        rsvpEndpoints.MapPut("/rsvp/{id}", async (Guid id, RSVP rsvp, IGenericAsyncDataService<RSVP, ApplicationDbContext> service) =>
        {
            var existingRsvp = await service.GetByIdAsync(id);

            if (existingRsvp is null)
                return Results.NotFound();

            existingRsvp.FirstName = rsvp.FirstName;
            existingRsvp.LastName = rsvp.LastName;
            existingRsvp.Email = rsvp.Email;
            existingRsvp.IsAttending = rsvp.IsAttending;
            existingRsvp.DietaryRestrictions = rsvp.DietaryRestrictions;
            existingRsvp.AccessibilityRequirements = rsvp.AccessibilityRequirements;
            existingRsvp.Pronouns = rsvp.Pronouns;
            existingRsvp.UpdatedAt = DateTime.UtcNow;

            var result = await service.UpdateAndSaveAsync(existingRsvp);
            return Results.Ok(result);
        })
        .WithName("UpdateRSVP")
        .WithDescription("Update an existing RSVP response");
    }
}