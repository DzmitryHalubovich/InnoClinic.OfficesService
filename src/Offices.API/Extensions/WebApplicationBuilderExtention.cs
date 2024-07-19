using FluentValidation;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Offices.Contracts.DTOs;
using Offices.Domain.Entities;
using Offices.Domain.Interfaces;
using Offices.Infrastructure.HttpClients;
using Offices.Infrastructure.Repositories;
using Offices.Presentation.Validators;
using Offices.Services.Abstractions;
using Offices.Services.Services;
using Serilog;

namespace Offices.API.Extensions;

public static class WebApplicationBuilderExtention
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IValidator<OfficeCreateDTO>, OfficeCreateValidator>();
        builder.Services.AddScoped<IValidator<OfficeUpdateDTO>, OfficeUpdateValidator>();
        builder.Services.AddScoped<IOfficesRepository, OfficesRepository>();
        builder.Services.AddScoped<IOfficesService, OfficesService>();

        builder.Services.AddHttpClient<DocumentsServiceHttpClient>();

        builder.Logging.ClearProviders();

        builder.Host.UseSerilog((ctx, lc) =>
            lc.WriteTo.Console()
            .ReadFrom.Configuration(ctx.Configuration));

        builder.Services.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = builder.Configuration["MongoDatabase:ConnectionString"];
            return new MongoClient(connectionString);
        });

        builder.Services.AddScoped(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var database = builder.Configuration["MongoDatabase:DatabaseName"];

            return client.GetDatabase(database)
                .GetCollection<Office>(builder.Configuration["MongoDatabase:OfficesCollectionName"]);
        });

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:5005";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "offices.api");
            });
        });

        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());

        builder.Services.AddSwaggerGen();
        builder.Services.AddAutoMapper(typeof(MapperProfile));

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(Presentation.Controllers.OfficesController).Assembly);

        builder.Services.AddEndpointsApiExplorer();
    }
}
