using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UndergroundBank.AccountService.Application.Configurations;
using UndergroundBank.AccountService.Infrastructure;
using UndergroundBank.AccountService.Infrastructure.MessageBroker;
using UndergroundBank.AccountService.Web.Configurations;
using UndergroundBank.Common.Configurations.JWT;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Helpers.TokenRequirment;
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
builder.Services.AddSwaggerConfiguration();

// Add business logic service dependencies
builder.Services.AddAccountBlServiceDependencies(builder.Configuration);

// Add Identity dependencies configuration
builder.Services.AddMicIdentityConfiguration();

// Application layer configuration
builder.Services.ConfigureApplicationLayer();

builder.Services.AddTokenRequirement();

builder.Services.UseJwtConfiguration(builder.Configuration);
builder.Services.QueueSubscribe();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetService<AccountDbContext>();
dbContext?.Database.Migrate();

//app.UseMiddleware<DefaultMiddleware>();

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var roles = Enum.GetNames(typeof(Role));
    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
    }
}

app.Run();
