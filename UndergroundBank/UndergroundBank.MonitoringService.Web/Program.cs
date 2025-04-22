using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using UndergroundBank.Common.Configurations.OpenIddict;
using UndergroundBank.Common.Middlewares;
using UndergroundBank.MonitoringService.Application.Helpers.Automapper;
using UndergroundBank.MonitoringService.Infrastructure;
using UndergroundBank.MonitoringService.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder
    .Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        var enumConverter = new JsonStringEnumConverter();
        opts.JsonSerializerOptions.Converters.Add(enumConverter);
    });

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenIddictValidation("https://localhost:5026/");
builder.Services.AddSwaggerWithOAuth();

builder.Services.AddCustomCors();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MonitoringServiceMapper));
builder.Services.AddMonitoringServiceConfiguration(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerWithOAuthUI();
}

try
{
    using var serviceScope = app.Services.CreateScope();
    var dbContext = serviceScope.ServiceProvider.GetService<MonitoringDbContext>();
    dbContext?.Database.Migrate();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating the database.");
    throw;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x =>
    x.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(origin => true)
);
app.UseMiddleware<DefaultMiddleware>();

//Ёто нужно дл€ эмул€ции нестабильной работы сервисов
//app.UseMiddleware<UnstableMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseCors("AllowSwaggerClients");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
