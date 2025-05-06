using Microsoft.EntityFrameworkCore;
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

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IUnitOfWork<ApplicationDbContext>, UnitOfWork<ApplicationDbContext>>();

        builder.Services.AddScoped<IGenericAsyncDataService<Invitation, ApplicationDbContext>, GenericAsyncDataService<Invitation, ApplicationDbContext>>();

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
