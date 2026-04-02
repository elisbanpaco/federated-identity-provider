using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using AuthServer.Host.Entities;

namespace AuthServer.Host.Controllers;

public class AuthorizationController : Controller
{
    private readonly UserManager<AppUser> _userManager;

    public AuthorizationController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    // Usamos el símbolo ~ para ignorar cualquier prefijo de ruta (como api/)
    // Esta ruta DEBE coincidir con lo que pusimos en Program.cs: options.SetTokenEndpointUris("/connect/token");
    [HttpPost("~/connect/token")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        // 1. Capturamos la petición mágica de OpenIddict
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("No se pudo obtener la petición de OpenID Connect.");

        // 2. Verificamos que el frontend esté usando el flujo de Contraseña (Login normal)
        if (request.IsPasswordGrantType())
        {
            // A. Buscar al usuario en la base de datos
            var user = await _userManager.FindByEmailAsync(request.Username ?? string.Empty);
            if (user == null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "El usuario o la contraseña son incorrectos."
                    }));
            }

            // B. Verificar que su cuenta esté Activa (¡Tu lógica de negocio brillando aquí!)
            if (!user.IsActive)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Tu cuenta institucional ha sido desactivada."
                    }));
            }

            // C. Verificar la contraseña con Identity
            var result = await _userManager.CheckPasswordAsync(user, request.Password ?? string.Empty);
            if (!result)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "El usuario o la contraseña son incorrectos."
                    }));
            }

            // D. Si todo es correcto, construimos el "Pasaporte" (Claims) del usuario
            var identity = new ClaimsIdentity(
                authenticationType: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                nameType: OpenIddictConstants.Claims.Name,
                roleType: OpenIddictConstants.Claims.Role);

            // Le agregamos sus datos personales al token
            identity.AddClaim(OpenIddictConstants.Claims.Subject, user.Id, OpenIddictConstants.Destinations.AccessToken);
            identity.AddClaim(OpenIddictConstants.Claims.Email, user.Email ?? string.Empty, OpenIddictConstants.Destinations.AccessToken);

            // Buscamos sus roles (Admin, Student) y los metemos al token
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                identity.AddClaim(OpenIddictConstants.Claims.Role, role, OpenIddictConstants.Destinations.AccessToken);
            }

            var principal = new ClaimsPrincipal(identity);

            // E. OpenIddict toma el "Pasaporte", lo firma criptográficamente y devuelve el JSON con el Token
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new NotImplementedException("Este tipo de login aún no está configurado.");
    }
}