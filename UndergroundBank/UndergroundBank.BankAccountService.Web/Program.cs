using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using UndergroundBank.BankAccountService.Application.Configurations;
using UndergroundBank.BankAccountService.Infrastructure;
using UndergroundBank.BankAccountService.Infrastructure.MessageBroker;
using UndergroundBank.BankAccountService.Infrastructure.Services;
using UndergroundBank.BankAccountService.Web.Configurations;
using UndergroundBank.Common.Configurations.JWT;
using UndergroundBank.Common.Configurations.OpenIddict;
using UndergroundBank.Common.Middlewares;

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
builder.Services.AddBankAccountServiceConfiguration(builder.Configuration);

// Application layer configuration
builder.Services.ConfigureBankAccountApplicationLayer();
builder.Services.QueueSubscribe();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerWithOAuthUI();
}

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetService<BankAccountDbContext>();
dbContext?.Database.Migrate();

app.UseCors("AllowSwaggerClients");

//app.UseMiddleware<DefaultMiddleware>();

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
