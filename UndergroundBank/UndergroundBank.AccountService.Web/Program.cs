using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using UndergroundBank.AccountService.Application.Configurations;
using UndergroundBank.AccountService.Infrastructure;
using UndergroundBank.AccountService.Infrastructure.MessageBroker;
using UndergroundBank.AccountService.Web.Configurations;
using UndergroundBank.Common.Configurations.OpenIddict;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Middlewares;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(
        5026,
        listenOptions =>
        {
            listenOptions.UseHttps();
        }
    );

    options.ListenLocalhost(
        7255,
        listenOptions =>
        {
            listenOptions.UseHttps();
        }
    );
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder
    .Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});
builder.Services.AddSwaggerWithOAuth();
builder.Services.AddCustomCors();
builder.Services.AddAccountBlServiceDependencies(builder.Configuration);
builder.Services.AddMicIdentityConfiguration();
builder.Services.ConfigureApplicationLayer();

builder.Services.QueueSubscribe();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerWithOAuthUI();
}

app.UseDeveloperExceptionPage();

app.UseForwardedHeaders();

app.UseHttpsRedirection();
app.UseSession();
app.UseRouting();
app.UseCors("AllowSwaggerClients");
app.UseMiddleware<DefaultMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.UseEndpoints(options =>
{
    options.MapControllers();
    options.MapDefaultControllerRoute();
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
    db.Database.Migrate();

    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    var existingApplication = await manager.FindByClientIdAsync("user-app");
    if (existingApplication != null)
    {
        await manager.DeleteAsync(existingApplication);
    }

    await manager.CreateAsync(
        new OpenIddictApplicationDescriptor
        {
            ClientId = "user-app",
            ClientSecret = "111",
            RedirectUris =
            {
                new Uri("https://localhost:7255/swagger/oauth2-redirect.html"),
                new Uri("https://localhost:7025/swagger/oauth2-redirect.html"),
                new Uri("https://localhost:7008/swagger/oauth2-redirect.html"),
                new Uri("https://localhost:7032/swagger/oauth2-redirect.html"),
                new Uri("http://localhost:5173/callback"),
                new Uri("http://localhost:5174/callback"),
            },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:7255/signout-callback-oidc"),
                new Uri("https://localhost:7025/signout-callback-oidc"),
                new Uri("https://localhost:7008/signout-callback-oidc"),
                new Uri("https://localhost:7032/signout-callback-oidc"),
                new Uri("http://localhost:5173"),
                new Uri("http://localhost:5174"),
            },
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.ResponseTypes.Code,
                Permissions.Prefixes.Scope + Scopes.OpenId,
                Permissions.Prefixes.Scope + Scopes.Profile,
                Permissions.Prefixes.Scope + Scopes.Email,
                Permissions.Prefixes.Scope + Scopes.Roles,
                Permissions.Prefixes.Scope + "api",
                Permissions.Prefixes.GrantType + GrantTypes.AuthorizationCode,
                Permissions.Prefixes.ResponseType + ResponseTypes.Code,
            },
            Requirements = { Requirements.Features.ProofKeyForCodeExchange },
        }
    );

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    foreach (var role in Enum.GetNames(typeof(Role)))
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
    }

    await IdentityDependenciesConfiguration.ConfigureAdminAsync(app.Services);
}

app.Run();
