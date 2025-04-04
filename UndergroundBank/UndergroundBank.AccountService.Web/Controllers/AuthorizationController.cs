using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using UndergroundBank.AccountService.Domain.Entities;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace UndergroundBank.AccountService.Web.Controllers;

[Route("api/[controller]"), ApiController]
public class AuthorizationController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IOpenIddictApplicationManager _applicationManager;

    public AuthorizationController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IOpenIddictApplicationManager applicationManager
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _applicationManager = applicationManager;
    }

    [HttpPost("~/connect/token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        ArgumentNullException.ThrowIfNull(request);

        if (request.IsAuthorizationCodeGrantType())
        {
            var result = await HttpContext.AuthenticateAsync(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
            );
            var principal = result.Principal;

            if (principal == null)
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            principal.SetDestinations(claim =>
                claim.Type switch
                {
                    Claims.Name => new[] { Destinations.AccessToken, Destinations.IdentityToken },
                    _ => new[] { Destinations.AccessToken },
                }
            );

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new InvalidOperationException("Unsupported grant type.");
    }

    [HttpGet("~/login")]
    public IActionResult Login([FromQuery] string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost("~/login")]
    public async Task<IActionResult> Login(
        [FromForm] string email,
        [FromForm] string password,
        [FromForm] string returnUrl
    )
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.LoginError = true;
            return View();
        }

        var principal = await _signInManager.CreateUserPrincipalAsync(user);
        HttpContext.Session.SetString("UserId", user.Id.ToString());
        HttpContext.Session.SetString("UserEmail", user.Email);
        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return Redirect("/");
    }

    [HttpGet("~/connect/authorize")]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        ArgumentNullException.ThrowIfNull(request);

        var redirectUri = request.RedirectUri;
        var state = request.State;

        var userId = HttpContext.Session.GetString("UserId");
        var userEmail = HttpContext.Session.GetString("UserEmail");

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
        {
            var query = HttpContext.Request.QueryString.ToString();
            return Redirect(
                $"/login?returnUrl={Uri.EscapeDataString($"/connect/authorize{query}")}"
            );
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.Email != userEmail)
        {
            return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(Claims.Subject, user.Id.ToString()),
        };

        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(
            claims,
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        );
        var claimsPrincipal = new ClaimsPrincipal(identity);
        claimsPrincipal.SetScopes(request.GetScopes());
        claimsPrincipal.SetResources("resource-server");
        foreach (var claim in claimsPrincipal.Claims)
        {
            claim.SetDestinations(
                claim.Type switch
                {
                    Claims.Name => new[] { Destinations.AccessToken, Destinations.IdentityToken },
                    _ => new[] { Destinations.AccessToken },
                }
            );
        }

        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
