using FluentValidation;
using MongoDB.Driver;
using Offices.Contracts.DTOs;
using Offices.Domain.Entities;
using Offices.Domain.Interfaces;
using Offices.Infrastructure.Repositories;
using Offices.Presentation.Validators;
using Offices.Services.Abstractions;
using Offices.Services.Services;

namespace Offices.API.Extensions;

public static class WebApplicationBuilderExtention
{
    public static void ConfigureScopes(this WebApplicationBuilder builder)
    {
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
        builder.Services.AddScoped<IValidator<OfficeCreateDTO>, OfficeCreateValidator>();
        builder.Services.AddScoped<IValidator<OfficeUpdateDTO>, OfficeUpdateValidator>();
        builder.Services.AddScoped<IOfficesRepository, OfficesRepository>();
        builder.Services.AddScoped<IOfficesService, OfficesService>();
    }

    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(typeof(MapperProfile));
        builder.Services.AddControllers()
            .AddApplicationPart(typeof(Offices.Presentation.Controllers.OfficesController).Assembly);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
            {
                Title = "OfficeAPI",
                Version = "v1"
            });

            var xmlFilename = Path.Combine(AppContext.BaseDirectory, "Offices.Presentation.xml");
            options.IncludeXmlComments(xmlFilename);
        });
    }
}
