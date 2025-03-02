using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using UndergroundBank.AccountService.Infrastructure;
using UndergroundBank.Common.Configurations.JWT;
using UndergroundBank.LoanService.Application.Configurations;
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
builder.Services.AddSwaggerConfiguration();

// Application layer configuration
builder.Services.ConfigureApplicationLayer();

// Add business logic service dependencies
builder.Services.AddLoanBlServiceDependencies(builder.Configuration);

builder.Services.AddTokenRequirement();

builder.Services.UseJwtConfiguration(builder.Configuration);
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetService<LoanDbContext>();
dbContext?.Database.Migrate();

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
