using WeddingAPI.Models;
using WeddingAPI.Services.Interfaces;
using WeddingAPI.Repository;
using WeddingAPI.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;

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

        rsvpGroup.MapPost("", async (IEnumerable<RSVP> rsvps, IGenericAsyncDataService<RSVP, ApplicationDbContext> service, IUnitOfWork<ApplicationDbContext> unitOfWork) =>
        {
            var rsvpsList = rsvps.ToList();
            
            foreach (var rsvp in rsvpsList)
            {
                await service.AddAsync(rsvp);
            }
            
            await unitOfWork.SaveChangesAsync(new CancellationToken());
            return Results.Created($"/api/rsvp", rsvpsList);
        })
        .WithName("CreateRSVPs")
        .WithDescription("Create multiple RSVP responses")
        .WithOpenApi(operation => {
            operation.RequestBody.Description = "Array of RSVP objects";
            var json = @"[
                  {
                    ""firstName"": ""John"",
                    ""lastName"": ""Doe"",
                    ""email"": ""john@example.com"",
                    ""isAttending"": true,
                    ""dietaryRestrictions"": ""Vegetarian"",
                    ""accessibilityRequirements"": null,
                    ""pronouns"": ""he/him""
                  }
                ]";
            operation.RequestBody.Content["application/json"].Example = new Microsoft.OpenApi.Any.OpenApiString(json);
            return operation;
        })
        .Produces<IEnumerable<RSVP>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        rsvpGroup.MapGet("", async (IGenericAsyncDataService<RSVP, ApplicationDbContext> service) =>
        {
            var rsvps = await service.GetAllAsync();
            return Results.Ok(rsvps);
        })
        .WithName("GetAllRSVPs")
        .WithDescription("Get all RSVP responses");

        rsvpGroup.MapGet("{id}", async (Guid id, IGenericAsyncDataService<RSVP, ApplicationDbContext> service) =>
        {
            var rsvp = await service.GetByIdAsync(id);
            return rsvp is null ? Results.NotFound() : Results.Ok(rsvp);
        })
        .WithName("GetRSVPById")
        .WithDescription("Get a specific RSVP response by ID");

        rsvpGroup.MapPut("{id}", async (Guid id, RSVP rsvp, IGenericAsyncDataService<RSVP, ApplicationDbContext> service) =>
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