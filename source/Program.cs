using WeddingAPI.Repository;
using WeddingAPI.Models;
using WeddingAPI.Services.Interfaces;
using WeddingAPI.Repository.Interfaces;
using WeddingAPI.Services;

namespace WeddingAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add DbContext
        //builder.Services.AddDbContext<ApplicationDbContext>(options =>
        //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Add UnitOfWork and DataService
        builder.Services.AddScoped<IUnitOfWork<ApplicationDbContext>, UnitOfWork<ApplicationDbContext>>();
        builder.Services.AddScoped<IGenericAsyncDataService<Invitation, ApplicationDbContext>, GenericAsyncDataService<Invitation, ApplicationDbContext>>();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // Map all endpoints
        WeddingApi.MapEndpoints(app);

        app.Run();
    }
}
