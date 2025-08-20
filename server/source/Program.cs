using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
// using System.Text;
using WeddingApi.Repository;
using WeddingApi.Models;
using WeddingApi.Services.Interfaces;
using WeddingApi.Repository.Interfaces;
using WeddingApi.Services;
using WeddingApi.Helpers;
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

            // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            // {
            //     Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
            //     Name = "Authorization",
            //     In = ParameterLocation.Header,
            //     Type = SecuritySchemeType.ApiKey,
            //     Scheme = "Bearer"
            // });

            // c.AddSecurityRequirement(new OpenApiSecurityRequirement
            // {
            //     {
            //         new OpenApiSecurityScheme
            //         {
            //             Reference = new OpenApiReference
            //             {
            //                 Type = ReferenceType.SecurityScheme,
            //                 Id = "Bearer"
            //             }
            //         },
            //         Array.Empty<string>()
            //     }
            // });
        });

        var dbSettings = new DatabaseSettings();

        builder.Configuration.GetSection("ConnectionStrings").Bind(dbSettings);

        builder.Services.AddSingleton(dbSettings);

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IUnitOfWork<ApplicationDbContext>, UnitOfWork<ApplicationDbContext>>();

        builder.Services.AddScoped<IGenericAsyncDataService<RSVP, ApplicationDbContext>, GenericAsyncDataService<RSVP, ApplicationDbContext>>();

        builder.Services.AddScoped<IFuzzyMatchService, FuzzyMatchService>();

        // var jwtSettings = new JwtSettings();
        // builder.Configuration.GetSection("JWT").Bind(jwtSettings);

        // builder.Services.AddSingleton(jwtSettings);

        // var clientCredentials = builder.Configuration.GetSection("ApiClients").Get<List<ClientCredentials>>() ?? new List<ClientCredentials>();

        // builder.Services.AddSingleton(clientCredentials);

        // builder.Services.AddScoped<ITokenService, TokenService>();

        // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //     .AddJwtBearer(options =>
        //     {
        //         options.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             ValidateIssuer = true,
        //             ValidateAudience = true,
        //             ValidateLifetime = true,
        //             ValidateIssuerSigningKey = true,
        //             ValidIssuer = jwtSettings.Issuer,
        //             ValidAudience = jwtSettings.Audience,
        //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
        //             RequireExpirationTime = true,
        //             ClockSkew = TimeSpan.FromMinutes(5)
        //         };
        //     });

        // builder.Services.AddAuthorization();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("WeddingClientPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("WeddingClientPolicy");

        // app.UseAuthentication();

        // app.UseAuthorization();

        EndpointMapper.MapEndpoints(app);

        app.Run();
    }
}
