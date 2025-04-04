using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using UndergroundBank.AccountService.Application.Configurations;
using UndergroundBank.AccountService.Infrastructure;
using UndergroundBank.AccountService.Web.Configurations;
using UndergroundBank.Common.Configurations.OpenIddict;
using UndergroundBank.Common.Data.Enums;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(
        5026,
        listenOptions =>
        {
            listenOptions.UseHttps(); // OpenIddict login, /connect/*
        }
    );

    options.ListenLocalhost(
        7255,
        listenOptions =>
        {
            listenOptions.UseHttps(); // Swagger + API
        }
    );
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Примерное время жизни сессии
});

// ✅ Контроллеры + Razor Pages + enum -> string
builder
    .Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddRazorPages();

// ✅ Swagger с OAuth2
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});
builder.Services.AddSwaggerWithOAuth();
builder.Services.AddAccountBlServiceDependencies(builder.Configuration);
builder.Services.AddMicIdentityConfiguration();
builder.Services.ConfigureApplicationLayer();

var app = builder.Build();

// ✅ Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Account Service API");
        c.OAuthClientId("user-app");
        c.OAuthClientSecret("111");
        c.OAuthUsePkce();
        c.OAuthScopes("openid", "profile", "email", "api");
    });
}

app.UseDeveloperExceptionPage();

app.UseForwardedHeaders();

app.UseHttpsRedirection();
app.UseSession();
app.UseRouting();
app.UseCors(x =>
    x.WithOrigins("https://localhost:7255").AllowAnyHeader().AllowAnyMethod().AllowCredentials()
);

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

    // Создаем новое приложение
    await manager.CreateAsync(
        new OpenIddictApplicationDescriptor
        {
            ClientId = "user-app",
            ClientSecret = "111",
            RedirectUris = { new Uri("https://localhost:7255/swagger/oauth2-redirect.html") },
            PostLogoutRedirectUris = { new Uri("https://localhost:7255/signout-callback-oidc") },
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

    // ✅ Роли + админ
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    foreach (var role in Enum.GetNames(typeof(Role)))
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
    }

    await IdentityDependenciesConfiguration.ConfigureAdminAsync(app.Services);
}

app.Run();
