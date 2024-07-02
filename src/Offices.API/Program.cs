using FluentValidation;
using Offices.API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) =>
    lc.WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

builder.Logging.ClearProviders();

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
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }