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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenIddictValidation("https://localhost:5026/");
builder.Services.AddSwaggerWithOAuth();
builder.Services.AddCustomCors();
builder.Services.AddBankAccountServiceConfiguration(builder.Configuration);
builder.Services.ConfigureBankAccountApplicationLayer();
builder.Services.QueueSubscribe();
builder.Services.AddHttpClient();

var app = builder.Build();

// Run database migrations
using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<BankAccountDbContext>();
    dbContext.Database.Migrate();

    // Ensure master bank account exists
    ServiceDependencyExtension.EnsureMasterBankAccountExists(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerWithOAuthUI();
}

app.UseCors("AllowSwaggerClients");

app.UseMiddleware<DefaultMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
