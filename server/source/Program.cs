using Microsoft.EntityFrameworkCore;
using WeddingApi.Repository;
using WeddingApi.Models;
using WeddingApi.Services.Interfaces;
using WeddingApi.Repository.Interfaces;
using WeddingApi.Services;
using Microsoft.OpenApi.Models;

namespace WeddingApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wedding API", Version = "v1" });
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

        app.UseHttpsRedirection();

        WeddingApi.MapEndpoints(app);
        
        app.Run();
    }
}
