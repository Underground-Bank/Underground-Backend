using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using UndergroundBank.Common.Configurations.JWT;
using UndergroundBank.Common.Configurations.OpenIddict;
using UndergroundBank.Common.Middlewares;
using UndergroundBank.HistoryService.Application.Configurations;
using UndergroundBank.HistoryService.Infrastructure;
using UndergroundBank.HistoryService.Infrastructure.Services;
using UndergroundBank.HistoryService.Infrastructure.Services.HistoryQueue;
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

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenIddictValidation("https://localhost:5026/");
builder.Services.AddSwaggerWithOAuth();

builder.Services.AddCustomCors();

builder.Services.AddHistoryBlServiceDependencies(builder.Configuration);

// Application layer configuration
builder.Services.ConfigureHistoryApplicationLayer();

builder.Services.AddSignalR();

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
    var dbContext = serviceScope.ServiceProvider.GetService<HistoryDbContext>();
    dbContext?.Database.Migrate();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating the database.");
    throw;
}

app.UseCors(x =>
    x.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(origin => true)
);

app.UseCors("AllowSwaggerClients");
app.UseMiddleware<DefaultMiddleware>();

app.UseHttpsRedirection();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<OperationHistoryHub>("/historyHub");

app.MapControllers();

app.Run();
