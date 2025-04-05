using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using UndergroundBank.Common.Configurations.JWT;
using UndergroundBank.Common.Configurations.OpenIddict;
using UndergroundBank.Common.Middlewares;
using UndergroundBank.LoanService.Application.Configurations;
using UndergroundBank.LoanService.Application.Interfaces;
using UndergroundBank.LoanService.Infrastructure;
using UndergroundBank.LoanService.Infrastructure.Services.LoanQueue;
using UndergroundBank.LoanService.Web.Configurations;

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

// Add business logic service dependencies
builder.Services.AddQuartzDependencies(builder.Configuration);
builder.Services.AddLoanBlServiceDependencies(builder.Configuration);

// Application layer configuration
builder.Services.ConfigureApplicationLayer();
builder.Services.AddListeners();
builder.Services.AddHttpClient();

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
    var dbContext = serviceScope.ServiceProvider.GetService<LoanDbContext>();
    dbContext?.Database.Migrate();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating the database.");
    throw;
}

using (var scope = app.Services.CreateScope())
{
    var jobScheduler = scope.ServiceProvider.GetRequiredService<IJobSchedulerService>();
    await jobScheduler.StartActiveJobsAsync();
}
app.UseCors("AllowSwaggerClients");
app.UseMiddleware<DefaultMiddleware>();

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
