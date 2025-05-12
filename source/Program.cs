using Microsoft.EntityFrameworkCore;
using WeddingAPI.Repository;
using WeddingAPI.Models;
using WeddingAPI.Services.Interfaces;
using WeddingAPI.Repository.Interfaces;
using WeddingAPI.Services;
using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Azure.AppConfiguration.AspNetCore;

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

        builder.Services.AddSwaggerGen();

        var dbSettings = new DatabaseSettings();

        builder.Configuration.GetSection("ConnectionStrings").Bind(dbSettings);
        
        builder.Services.AddSingleton(dbSettings);

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IUnitOfWork<ApplicationDbContext>, UnitOfWork<ApplicationDbContext>>();

        builder.Services.AddScoped<IGenericAsyncDataService<Invitation, ApplicationDbContext>, GenericAsyncDataService<Invitation, ApplicationDbContext>>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Use Azure App Configuration middleware to refresh configuration
        if (!string.IsNullOrEmpty(appConfigConnectionString))
        {
            app.UseAzureAppConfiguration();
        }

        app.UseHttpsRedirection();
        WeddingApi.MapEndpoints(app);
        app.Run();
    }
}
