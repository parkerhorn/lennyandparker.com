using Microsoft.EntityFrameworkCore;
using WeddingAPI.Repository;
using WeddingAPI.Models;
using WeddingAPI.Services.Interfaces;
using WeddingAPI.Repository.Interfaces;
using WeddingAPI.Services;
using Microsoft.OpenApi.Models;

namespace WeddingAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        var appConfigConnectionString = builder.Configuration["AppConfiguration:ConnectionString"];

        if (!string.IsNullOrEmpty(appConfigConnectionString))
        {
            builder.Configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(appConfigConnectionString)
                       .Select("ConnectionStrings:*")
                       .ConfigureRefresh(refresh =>
                       {
                           refresh.Register("ConnectionStrings:DefaultConnection", refreshAll: true)
                                  .SetRefreshInterval(TimeSpan.FromMinutes(5));
                       });
            });
        }

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "Wedding API", 
                Version = "v1" 
            });
            
            // Add example for RSVP array
            c.MapType<IEnumerable<RSVP>>(() => new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = "RSVP"
                    }
                },
                Example = new Microsoft.OpenApi.Any.OpenApiArray
                {
                    new Microsoft.OpenApi.Any.OpenApiObject
                    {
                        ["firstName"] = new Microsoft.OpenApi.Any.OpenApiString("John"),
                        ["lastName"] = new Microsoft.OpenApi.Any.OpenApiString("Doe"),
                        ["email"] = new Microsoft.OpenApi.Any.OpenApiString("john@example.com"),
                        ["isAttending"] = new Microsoft.OpenApi.Any.OpenApiBoolean(true),
                        ["dietaryRestrictions"] = new Microsoft.OpenApi.Any.OpenApiString("Vegetarian"),
                        ["pronouns"] = new Microsoft.OpenApi.Any.OpenApiString("he/him")
                    }
                }
            });
        });

        var dbSettings = new DatabaseSettings();

        builder.Configuration.GetSection("ConnectionStrings").Bind(dbSettings);
        
        builder.Services.AddSingleton(dbSettings);

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IUnitOfWork<ApplicationDbContext>, UnitOfWork<ApplicationDbContext>>();

        builder.Services.AddScoped<IGenericAsyncDataService<RSVP, ApplicationDbContext>, GenericAsyncDataService<RSVP, ApplicationDbContext>>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (!string.IsNullOrEmpty(appConfigConnectionString))
        {
            app.UseAzureAppConfiguration();
        }

        app.UseHttpsRedirection();
        WeddingApi.MapEndpoints(app);
        app.Run();
    }
}
