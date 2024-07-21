using FluentValidation;
using Offices.API.Extensions;
using Offices.Services.Abstractions;
using Offices.Services.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "OfficesCatalog_"; 
});

builder.Services.AddScoped<IRedisCahceService, RedisCacheService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
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
app.MapControllers()
    .RequireAuthorization("ApiScope");

app.Run();

public partial class Program { }