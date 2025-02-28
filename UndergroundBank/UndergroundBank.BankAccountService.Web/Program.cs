using Microsoft.EntityFrameworkCore;
using UndergroundBank.BankAccountService.Infrastructure;
using UndergroundBank.BankAccountService.Web.Configurations;
using UndergroundBank.Common.Configurations.JWT;
using UndergroundBank.Common.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add business logic service dependencies
builder.Services.AddBankAccountServiceConfiguration(builder.Configuration);

// Application layer configuration
//builder.Services.ConfigureApplicationLayer();

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
var dbContext = serviceScope.ServiceProvider.GetService<BankAccountDbContext>();
dbContext?.Database.Migrate();

app.UseMiddleware<DefaultMiddleware>();

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
