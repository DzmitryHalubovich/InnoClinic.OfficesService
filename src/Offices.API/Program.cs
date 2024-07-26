using FluentValidation;
using MassTransit;
using Offices.API.Extensions;
using Offices.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var MTRabbitMqOptions = builder.Configuration
    .GetSection("MassTransitRabbitMq")
    .Get<MassTransitRabbitMqConfiguration>();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(MTRabbitMqOptions.Host,"/", h =>
        {
            h.Username(MTRabbitMqOptions.Username);
            h.Password(MTRabbitMqOptions.Password);
        });

        cfg.ConfigureEndpoints(context);

        cfg.AutoDelete = true;
    });
});

builder.ConfigureServices();

ValidatorOptions.Global.LanguageManager.Enabled = false;

var app = builder.Build();

var logger = app.Services.GetRequiredService<Serilog.ILogger>();
app.ConfigureExceptionHandler(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSession();
app.MapHealthChecks("/_health");
app.UseAuthorization();
app.MapControllers();
    //.RequireAuthorization("ApiScope");

app.Run();

public partial class Program { }